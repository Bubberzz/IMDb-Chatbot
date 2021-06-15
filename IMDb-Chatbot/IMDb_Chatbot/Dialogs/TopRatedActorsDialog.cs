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
    public class TopRatedActorsDialog : CancelAndHelpDialog
    {
        private static IImdbService _imdbService;
        private const string ShowMoreText = "Show More: Top rated actors";

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
            ImdbResult.TopRatedActors = await _imdbService.GetTopRatedActors();
            var actorsList = new List<TopRatedActors.Root>();
            for (var i = 0; i < 10; i++)
            {
                actorsList.Add(ImdbResult.TopRatedActors[i]);
            }

            var card = new Cards(_imdbService).CreateTopRatedActorCardAsync(actorsList, false);
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
            var actorsList = new List<TopRatedActors.Root>();
            var reply = MessageFactory.Attachment(new List<Attachment>());

            for (var i = Counter.MinCount; i < Counter.MaxCount; i++)
            {
                actorsList.Add(ImdbResult.TopRatedActors[i]);
            }

            if (Counter.MaxCount >= 80)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Loading.."),
                    cancellationToken);
                var cardsClass = new Cards(_imdbService).CreateTopRatedActorCardAsync(actorsList, true);
                reply.Attachments.Add(cardsClass.Result.ToAttachment());
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Loading.."),
                    cancellationToken);
                var cardsClass = new Cards(_imdbService).CreateTopRatedActorCardAsync(actorsList, false);
                reply.Attachments.Add(cardsClass.Result.ToAttachment());
                Counter.MinCount += 10;
                Counter.MaxCount += 10;
            }
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);
        }
    }
}