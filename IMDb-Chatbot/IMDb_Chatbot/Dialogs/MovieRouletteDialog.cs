using System;
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
    public class MovieRouletteDialog : CancelAndHelpDialog
    {
        private static IImdbService _imdbService;
        private IGenerateFilmId _generateFilmId;
        private string _filmId;
        private AutoComplete.Root _filmTitle;
        private ImdbSearch.Root _imdbResult;


        public MovieRouletteDialog(
            IImdbService imdbService)
            : base(nameof(MovieRouletteDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                DisplayLoadingMessage,
                GenerateIMDbID,
                FilterResults,
                ReturnResults
            }));

            InitialDialogId = nameof(WaterfallDialog);
            _imdbService = imdbService;
            _imdbResult = new ImdbSearch.Root();
            _generateFilmId = new GenerateFilmId();
        }


        private async Task<DialogTurnResult> DisplayLoadingMessage(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            // Display loading message
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Finding something to watch.. this may take a moment"),
                cancellationToken);
            return await stepContext.NextAsync(_imdbResult, cancellationToken);
        }

        private async Task<DialogTurnResult> GenerateIMDbID(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Generate random IMDb IDs until we find one that has a title
            _filmTitle = new AutoComplete.Root();
            while (_filmTitle.d is null)
            {
                _filmId = string.Empty;
                _filmId = _generateFilmId.FilmId();
                _filmTitle = await _imdbService.AutoComplete(_filmId);
            }

            // Query IMDb by film title
            stepContext.Context.Activity.Value = _filmTitle.d[0].l;
            stepContext.Context.Activity.Text = "IMDb Roulette";
            return await stepContext.BeginDialogAsync(nameof(ImdbSearchDialog),
                new ImdbSearchDialog(_imdbService), cancellationToken);
        }

        private async Task<DialogTurnResult> FilterResults(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            // Assign the result from ImdbSearchDialog to a variable
            _imdbResult = (ImdbSearch.Root) stepContext.Result;

            if (_imdbResult is null)
            {
                return await stepContext.NextAsync(_imdbResult, cancellationToken);
            }

            // Filter out unwanted results
            while (_imdbResult.results[0].genre is "Adult" ||
                   _imdbResult.results[0].titleType is not "movie"
            )
            {
                // Return to previous step and generate a new film ID
                stepContext.ActiveDialog.State["stepIndex"] = (int)stepContext.ActiveDialog.State["stepIndex"] - 2;
                return await stepContext.NextAsync(null, cancellationToken);
            }

            return await stepContext.NextAsync(_imdbResult, cancellationToken);
        }

        private async Task<DialogTurnResult> ReturnResults(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            if (_imdbResult is null)
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }

            var card = Cards.GetCard(_imdbResult, false, true);
            var reply = MessageFactory.Attachment(new List<Attachment>()
            {
                card[0].ToAttachment()
            });

            // Send the card(s) to the user as an attachment to the activity
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return await stepContext.NextAsync(null, cancellationToken);
        }
    }
}