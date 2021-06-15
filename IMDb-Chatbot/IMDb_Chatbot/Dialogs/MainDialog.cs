using IMDb_Chatbot.CognitiveModels;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CardsBot.Models;
using IMDb_Chatbot.Interfaces;
using IMDb_Chatbot.Models;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.BotBuilderSamples;

namespace IMDb_Chatbot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        protected readonly ILogger _logger;
        private IImdbService _imdbService;
        private IImdbResult _imdbResult;


        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(
            TopRatedMoviesDialog topRatedMoviesDialog,
            TopRatedActorsDialog topRatedActorsDialog,
            ComingSoonMoviesDialog comingSoonMoviesDialog,
            //Dialog movieRouletteDialog,
            ILogger<MainDialog> logger,
            IImdbService imdbService,
            IImdbResult imdbResult)
            : base(nameof(MainDialog))
        {
            _logger = logger;
            _imdbService = imdbService;
            _imdbResult = imdbResult;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(topRatedMoviesDialog);
            AddDialog(topRatedActorsDialog);
            AddDialog(comingSoonMoviesDialog);
            //AddDialog(movieRouletteDialog);
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
                        //return await stepContext.BeginDialogAsync(nameof(movieRouletteDialog), new MovieRouletteDialog(_imdbService), cancellationToken);
                        break;
                }
            }

            // Create a response card based on activity received from user
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

                default:
                {
                    //_imdbResult = await _getImdbResult.ImdbResult(input);

                    //var cards = Cards.GetHeroCard((ImdbResult)_imdbResult, true, false);
                    //reply.Attachments.Add(cards[0].ToAttachment());

                    break;
                }
            }

            _logger.LogInformation("MainDialog.ReturnResultCard");
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            // Give the user instructions about what to do next
            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text("Type anything to do another search or type 'Options'"), cancellationToken);

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private static IList<Choice> GetChoices()
        {
            var cardOptions = new List<Choice>()
            {
                new Choice() {Value = "Top rated movies", Synonyms = new List<string>() {"rated movies"}},
                new Choice()
                    {Value = "Top rated actors", Synonyms = new List<string>() {"actor", "actors", "rated actors"}},
                new Choice() {Value = "Coming soon movies", Synonyms = new List<string>() {"coming", "soon"}},
                new Choice() {Value = "IMDb Roulette", Synonyms = new List<string>() {"roulette", "imdb"}},
            };

            return cardOptions;
        }
    }
}