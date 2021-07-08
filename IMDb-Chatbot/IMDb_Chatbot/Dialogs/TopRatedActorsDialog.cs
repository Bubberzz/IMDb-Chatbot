using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IMDb_Chatbot.Interfaces;
using IMDb_Chatbot.Models;
using IMDb_Chatbot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace IMDb_Chatbot.Dialogs
{
    public class TopRatedActorsDialog : CancelAndHelpDialog
    {
        private static IImdbService _imdbService;
        private const string ShowMoreText = "Show More: Top rated actors";
        private List<TopRatedActors.Root> _fullActorsList;
        private List<TopRatedActors.Root> _actorsListToDisplayToUser;

        public TopRatedActorsDialog(IImdbService imdbService)
            : base(nameof(TopRatedActorsDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                GetActorsList,
                ShowMoreResults
            }));

            InitialDialogId = nameof(WaterfallDialog);
            _imdbService = imdbService;
        }

        public async Task<DialogTurnResult> GetActorsList(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            if (stepContext.Context.Activity.Value is not null and ShowMoreText)
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Loading.."), cancellationToken);
            _fullActorsList = await _imdbService.GetTopRatedActors();
            _actorsListToDisplayToUser = new List<TopRatedActors.Root>();
            for (var i = 0; i < 10; i++)
            {
                _actorsListToDisplayToUser.Add(_fullActorsList[i]);
            }

            var card = new Cards(_imdbService).CreateTopRatedActorCardAsync(_actorsListToDisplayToUser, false);
            var reply = MessageFactory.Attachment(new List<Attachment>()
            {
                card.Result.ToAttachment()
            });

            Counter.MinCount = 10;
            Counter.MaxCount = 20;

            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> ShowMoreResults(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Context.Activity.Value is not (not null and ShowMoreText))
                return await stepContext.NextAsync(null, cancellationToken);
            _actorsListToDisplayToUser = new List<TopRatedActors.Root>();
            var reply = MessageFactory.Attachment(new List<Attachment>());

            for (var i = Counter.MinCount; i < Counter.MaxCount; i++)
            {
                _actorsListToDisplayToUser.Add(_fullActorsList[i]);
            }

            if (Counter.MaxCount >= 80)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Loading.."),
                    cancellationToken);
                var cardsClass = new Cards(_imdbService).CreateTopRatedActorCardAsync(_actorsListToDisplayToUser, true);
                reply.Attachments.Add(cardsClass.Result.ToAttachment());
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Loading.."),
                    cancellationToken);
                var cardsClass = new Cards(_imdbService).CreateTopRatedActorCardAsync(_actorsListToDisplayToUser, false);
                reply.Attachments.Add(cardsClass.Result.ToAttachment());
                Counter.MinCount += 10;
                Counter.MaxCount += 10;
            }
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);
        }
    }
}