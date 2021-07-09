using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IMDb_Chatbot.Interfaces;
using IMDb_Chatbot.Services;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace IMDb_Chatbot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        protected readonly ILogger _logger;
        private IImdbService _imdbService;
        private string _recommendMovie;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(
            TopRatedMoviesDialog topRatedMoviesDialog,
            TopRatedActorsDialog topRatedActorsDialog,
            ComingSoonMoviesDialog comingSoonMoviesDialog,
            MovieRouletteDialog movieRouletteDialog,
            ImdbSearchDialog imdbSearchDialog,
            ILogger<MainDialog> logger,
            IImdbService imdbService)
            : base(nameof(MainDialog))
        {
            _logger = logger;
            _imdbService = imdbService;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(topRatedMoviesDialog);
            AddDialog(topRatedActorsDialog);
            AddDialog(comingSoonMoviesDialog);
            AddDialog(imdbSearchDialog);
            AddDialog(movieRouletteDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                SelectOptionAsync,
                ReturnResultCard,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> SelectOptionAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            if (stepContext.Context.Activity.Text == "Options")
            {
                var options = new PromptOptions()
                {
                    Prompt = MessageFactory.Text("Pick one of the following options"),
                    RetryPrompt =
                        MessageFactory.Text("That was not a valid choice, please one of the below options."),
                    Choices = GetChoices(),
                };

                // Prompt the user with the configured PromptOptions.
                return await stepContext.PromptAsync(nameof(ChoicePrompt), options, cancellationToken);
            }

            _logger.LogInformation("MainDialog.SelectOptionAsync");
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> ReturnResultCard(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("MainDialog.ReturnResultCard");

            // Return recommended movie if the user has responded to the question with a yes or no
            switch (_recommendMovie)
            {
                case "Yes" when UserResponse.UserResponsePositive().Contains(stepContext.Context.Activity.Text):
                {
                    await stepContext.Context.SendActivityAsync(
                        MessageFactory.Text("Generating a personalised recommendation.."), cancellationToken);
                    Thread.Sleep(10500);
                    var card = Cards.GetRickAstleyCard();
                    var reply = MessageFactory.Attachment(new List<Attachment>()
                    {
                        card.ToAttachment()
                    });
                    _recommendMovie = null;
                    await stepContext.Context.SendActivityAsync(
                        MessageFactory.Text("I think you would like this movie."), cancellationToken);
                    await stepContext.Context.SendActivityAsync(reply, cancellationToken);
                    return await stepContext.NextAsync(null, cancellationToken);

                }
                case "Yes" when UserResponse.UserResponseNegative().Contains(stepContext.Context.Activity.Text):
                {
                    _recommendMovie = null;
                    return await stepContext.NextAsync(null, cancellationToken);
                }
            }

            // Create a response based on the options user selected in previous step
            if (stepContext.Result is not null)
            {
                switch (((FoundChoice) stepContext.Result).Value)
                {
                    case "Top rated movies":
                        return await stepContext.BeginDialogAsync(nameof(TopRatedMoviesDialog),
                            new TopRatedMoviesDialog(_imdbService), cancellationToken);

                    case "Top rated actors":
                        return await stepContext.BeginDialogAsync(nameof(TopRatedActorsDialog),
                            new TopRatedActorsDialog(_imdbService), cancellationToken);
                    case "Coming soon movies":
                        return await stepContext.BeginDialogAsync(nameof(ComingSoonMoviesDialog),
                            new ComingSoonMoviesDialog(_imdbService), cancellationToken);

                    case "IMDb Roulette":
                        return await stepContext.BeginDialogAsync(nameof(MovieRouletteDialog),
                            new MovieRouletteDialog(_imdbService), cancellationToken);
                }
            }

            // Create a response based on activity received from user
            switch (stepContext.Context.Activity.Value)
            {
                case not null and "Show More: Top rated movies":
                {
                    return await stepContext.BeginDialogAsync(nameof(TopRatedMoviesDialog),
                        new TopRatedMoviesDialog(_imdbService), cancellationToken);
                }

                case not null and "Show More: Top rated actors":
                {
                    return await stepContext.BeginDialogAsync(nameof(TopRatedActorsDialog),
                        new TopRatedActorsDialog(_imdbService), cancellationToken);
                }

                case not null and "Show More: Coming Soon movies":
                {
                    return await stepContext.BeginDialogAsync(nameof(ComingSoonMoviesDialog),
                        new ComingSoonMoviesDialog(_imdbService), cancellationToken);
                }

                case not null and "IMDb Roulette":
                {
                    return await stepContext.BeginDialogAsync(nameof(MovieRouletteDialog),
                        new MovieRouletteDialog(_imdbService),
                        cancellationToken);
                }

                case not null and "Show More: Movie Results":
                {
                    return await stepContext.BeginDialogAsync(nameof(ImdbSearchDialog),
                        new ImdbSearchDialog(_imdbService), cancellationToken);
                }

                default:
                {
                    if (stepContext.Context.Activity.Text != null &&
                        stepContext.Context.Activity.Text.Contains("recommend"))
                    {
                        await stepContext.Context.SendActivityAsync(
                            MessageFactory.Text(
                                "Would you like me to recommend a movie based on your searches? (ALPHA AI machine learning technology)"),
                            cancellationToken);
                        _recommendMovie = "Yes";
                        return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
                    }

                    stepContext.Context.Activity.Value = "";
                    return await stepContext.BeginDialogAsync(nameof(ImdbSearchDialog),
                        new ImdbSearchDialog(_imdbService), cancellationToken);
                }
            }
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            // Give the user instructions about what to do next
            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text("Type anything to do another search or type 'Options'"), cancellationToken);
            _logger.LogInformation("MainDialog.FinalStepAsync");
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private static IList<Choice> GetChoices()
        {
            var cardOptions = new List<Choice>()
            {
                new() {Value = "Top rated movies", Synonyms = new List<string>() {"rated movies", "1"}},
                new() {Value = "Top rated actors", Synonyms = new List<string>() {"actor", "actors", "rated actors", "2"}},
                new() {Value = "Coming soon movies", Synonyms = new List<string>() {"coming", "soon", "3"}},
                new() {Value = "IMDb Roulette", Synonyms = new List<string>() {"roulette", "imdb", "4"}},
            };
            return cardOptions;
        }
    }
}
