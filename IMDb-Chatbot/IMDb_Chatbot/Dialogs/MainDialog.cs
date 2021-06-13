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
using IMDb_Chatbot.Interfaces;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace IMDb_Chatbot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly FlightBookingRecognizer _luisRecognizer;
        protected readonly ILogger _logger;
        private IImdbService _imdbService;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(
            FlightBookingRecognizer luisRecognizer, 
            Dialog topRatedMoviesDialog,
            Dialog topRatedActorsDialog,
            Dialog comingSoonMoviesDialog,
            Dialog movieRouletteDialog,
            ILogger<MainDialog> logger,
            IImdbService imdbService)
            : base(nameof(MainDialog))
        {
            _luisRecognizer = luisRecognizer;
            _logger = logger;
            _imdbService = imdbService;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(topRatedMoviesDialog);
            //AddDialog(topRatedActorsDialog);
            //AddDialog(comingSoonMoviesDialog);
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

        private async Task<DialogTurnResult> SelectOptionAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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

            _logger.LogInformation("MainDialog.ChoiceCardStepAsync");
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> ReturnResultCard(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Cards are sent as Attachments in the Bot Framework. Create a reply activity to return to the user.
            var reply = MessageFactory.Attachment(new List<Attachment>());

            // Store user's text input in a variable
            var input = stepContext.Context.Activity.Text;

            if (stepContext.Result is not null)
            {
                switch (((FoundChoice)stepContext.Result).Value)
                {
                    case "Top rated movies":
                        return await stepContext.BeginDialogAsync(nameof(TopRatedMoviesDialog), new TopRatedMoviesDialog(_imdbService), cancellationToken);

                    case "Top rated actors":
                        //return await stepContext.BeginDialogAsync(nameof(topRatedActorsDialog), new TopRatedActorsDialog(_imdbService), cancellationToken);
                        break;
                    case "Coming soon movies":
                        //return await stepContext.BeginDialogAsync(nameof(comingSoonMoviesDialog), new ComingSoonMoviesDialog(_imdbService), cancellationToken);
                        break;
                    case "IMDb Roulette":
                        //return await stepContext.BeginDialogAsync(nameof(movieRouletteDialog), new MovieRouletteDialog(_imdbService), cancellationToken);
                        break;
                }
            }

            // Send the card(s) to the user as an attachment to the activity
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
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
                new Choice() {Value = "Top rated actors", Synonyms = new List<string>() {"actor", "actors", "rated actors"}},
                new Choice() {Value = "Coming soon movies", Synonyms = new List<string>() {"coming", "soon"}},
                new Choice() {Value = "IMDb Roulette", Synonyms = new List<string>() {"roulette", "imdb"}},
            };

            return cardOptions;
        }
    }
}
