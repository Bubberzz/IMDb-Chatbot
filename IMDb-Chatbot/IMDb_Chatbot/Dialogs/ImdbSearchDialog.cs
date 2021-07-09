using System.Collections.Generic;
using System.Linq;
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
    public class ImdbSearchDialog : CancelAndHelpDialog
    {
        private const string ShowMoreText = "Show More: Movie Results";
        private const string IMDbReoulette = "IMDb Roulette";
        private static IImdbService _imdbService;
        private readonly IGetActorDetails _getActorDetails;
        private readonly IGetFilmDetails _getFilmDetails;
        private ImdbSearch.Root _response;


        public ImdbSearchDialog(IImdbService imdbService)
            : base(nameof(ImdbSearchDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ReturnSingleSearch,
                ReturnMultipleSearches,
                ReturnSearchForIMDbRoulette
            }));

            InitialDialogId = nameof(WaterfallDialog);
            _imdbService = imdbService;
            _getFilmDetails = new GetFilmDetails(_imdbService);
            _getActorDetails = new GetActorDetails(_imdbService);
            _response = new ImdbSearch.Root();
        }

        private async Task<DialogTurnResult> ReturnSingleSearch(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            if (stepContext.Context.Activity.Text == IMDbReoulette ||
                stepContext.Context.Activity.Value.ToString() == ShowMoreText)
                return await stepContext.NextAsync(null, cancellationToken);

            var search = stepContext.Context.Activity.Text.Replace(" ", "%20");
            _response = await _imdbService.GetSearchResult(search);

            if (_response.results is null)
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text(@"Sorry I couldn't find anything ¯\_(ツ)_/¯"),
                    cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Please try again"),
                    cancellationToken);
                return await stepContext.NextAsync(null, cancellationToken);
            }

            var resultId = ExtractResultId.GetId(_response);

            if (_response.results[0].id.StartsWith("/title")) await _getFilmDetails.GetDetails(_response, resultId);

            if (_response.results[0].id.StartsWith("/name")) await _getActorDetails.GetDetails(_response, resultId);

            var card = Cards.GetCard(_response, true, false);
            var reply = MessageFactory.Attachment(new List<Attachment>
            {
                card[0].ToAttachment()
            });

            // Send the card(s) to the user as an attachment to the activity
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ReturnMultipleSearches(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            if (stepContext.Context.Activity.Value.ToString() != ShowMoreText)
                return await stepContext.NextAsync(null, cancellationToken);

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Loading.."),
                cancellationToken);

            for (var i = 1; i < _response.results.Count; i++)
            {
                var resultId = ExtractResultId.GetId(_response, i);

                if (_response.results[i].id.StartsWith("/title"))
                    await _getFilmDetails.GetDetails(_response, resultId, i);

                if (_response.results[i].id.StartsWith("/name"))
                    await _getActorDetails.GetDetails(_response, resultId, i);
            }

            var cards = Cards.GetCard(_response, false, false);
            var reply = MessageFactory.Attachment(new List<Attachment>());
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            for (var i = 1; i < cards.Count; i++) reply.Attachments.Add(cards[i].ToAttachment());

            // Send the card(s) to the user as an attachment to the activity
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> ReturnSearchForIMDbRoulette(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            if (stepContext.Context.Activity.Text != IMDbReoulette)
                return await stepContext.NextAsync(null, cancellationToken);

            var search = stepContext.Context.Activity.Value.ToString()?.Replace(" ", "%20");

            // Remove characters that can cause problems
            var charsToRemove = new[] {"@", "#"};
            search = charsToRemove.Aggregate(search, (current, c) => current.Replace(c, string.Empty));
            var response = await _imdbService.GetSearchResult(search);


            if (response.results is null)
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text(@"Sorry I couldn't find anything ¯\_(ツ)_/¯"),
                    cancellationToken);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("Please try again"),
                    cancellationToken);
                return await stepContext.NextAsync(null, cancellationToken);
            }

            var resultId = ExtractResultId.GetId(response);
            await _getFilmDetails.GetDetails(response, resultId);
            return await stepContext.NextAsync(response, cancellationToken);
        }
    }
}