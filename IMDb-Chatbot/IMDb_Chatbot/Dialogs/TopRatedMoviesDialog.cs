using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CardsBot.Models;
using IMDb_Chatbot.Interfaces;
using IMDb_Chatbot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;

namespace IMDb_Chatbot.Dialogs
{
    public class TopRatedMoviesDialog : CancelAndHelpDialog
    {
        private static IImdbService _imdbService;

        public TopRatedMoviesDialog(IImdbService imdbService)
            : base(nameof(TopRatedMoviesDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                MoviesList,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);

            _imdbService = imdbService;
        }

        private async Task<DialogTurnResult> MoviesList(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Loading.."), cancellationToken);

            ImdbResult.TopRatedMovies = await _imdbService.GetTopRatedMovies();

            var firstBatch = new List<CardsBot.Models.TopRatedMovies.Root>();

            for (var i = 0; i < 10; i++)
            {
                firstBatch.Add(ImdbResult.TopRatedMovies[i]);
            }

            Counter.MinCount = 0;
            Counter.MaxCount = 10;

            var reply = MessageFactory.Attachment(new List<Attachment>());
            var cardsClass = new Cards(_imdbService).CreateTopRatedMovieCardAsync(firstBatch, false);
            //var cards = Cards.CreateTopRatedFilmCard(result);
            reply.Attachments.Add(cardsClass.Result.ToAttachment());


            Counter.MinCount += 10;
            Counter.MaxCount += 10;

            // break;
            // Send the card(s) to the user as an attachment to the activity
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            // Give the user instructions about what to do next
            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text("Type anything to do another search or type 'Options'"), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

        }
    }
}