using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IMDb_Chatbot.Interfaces;
using IMDb_Chatbot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace IMDb_Chatbot.Dialogs
{
    public class ComingSoonMoviesDialog : CancelAndHelpDialog
    {
        private static IImdbService _imdbService;
        private const string ShowMoreText = "Show More: Coming Soon movies";
        private List<string> _fullMoviesList;
        private List<string> _moviesToDisplayToUser;
        public ComingSoonMoviesDialog(IImdbService imdbService)
            : base(nameof(ComingSoonMoviesDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                GetMoviesList,
                ShowMoreResults
            }));

            InitialDialogId = nameof(WaterfallDialog);
            _imdbService = imdbService;
        }

        private async Task<DialogTurnResult> GetMoviesList(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Check of this is a first request or a request to see more results from the list
            if (stepContext.Context.Activity.Value is not null and ShowMoreText)
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }

            // Display loading message
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Loading.."), cancellationToken);

            // Call IMDb service and save into model
            _fullMoviesList = await _imdbService.ComingSoon();

            _moviesToDisplayToUser = new List<string>();

            for (var i = 0; i < 10; i++)
            {
                _moviesToDisplayToUser.Add(_fullMoviesList[i]);
            }

            // Update the movie counter - allows show more results to display next 10 in the list
            Counter.MinCount = 10;
            Counter.MaxCount = 20;

            var card = new Cards(_imdbService).CreateComingSoonCardAsync(_moviesToDisplayToUser, false);
            var reply = MessageFactory.Attachment(new List<Attachment>()
            {
                card.Result.ToAttachment()
            });

            // Send the card(s) to the user as an attachment to the activity
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ShowMoreResults(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            if (stepContext.Context.Activity.Value is not (not null and ShowMoreText))
                return await stepContext.NextAsync(null, cancellationToken);
            _moviesToDisplayToUser = new List<string>();
            var reply = MessageFactory.Attachment(new List<Attachment>());

            for (var i = Counter.MinCount; i < Counter.MaxCount; i++)
            {
                _moviesToDisplayToUser.Add(_fullMoviesList[i]);
            }

            if (Counter.MaxCount >= 30)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Loading.."),
                    cancellationToken);
                var card = new Cards(_imdbService).CreateComingSoonCardAsync(_moviesToDisplayToUser, true);
                reply.Attachments.Add(card.Result.ToAttachment());
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Loading.."),
                    cancellationToken);
                var card = new Cards(_imdbService).CreateComingSoonCardAsync(_moviesToDisplayToUser, false);
                reply.Attachments.Add(card.Result.ToAttachment());
                Counter.MinCount += 10;
                Counter.MaxCount += 10;
            }

            // Send the card(s) to the user as an attachment to the activity
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);
        }
    }
}