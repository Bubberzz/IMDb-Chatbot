using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace IMDb_Chatbot.Bots
{
    public class DialogAndWelcomeBot<T> : DialogBot<T>
        where T : Dialog
    {
        public DialogAndWelcomeBot(ConversationState conversationState, UserState userState, T dialog,
            ILogger<DialogBot<T>> logger)
            : base(conversationState, userState, dialog, logger)
        {
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded,
            ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var card = new HeroCard
                    {
                        Title = "Welcome to IMDb bot!",
                        Text = @"Search for a film or actor or type 'Options' for more options",
                        Images = new List<CardImage>
                            {new("https://raw.githubusercontent.com/Bubberzz/IMDb-Chatbot/main/Images/Imdb-bot.jpg")}
                    };

                    var response = MessageFactory.Attachment(card.ToAttachment());
                    await turnContext.SendActivityAsync(response, cancellationToken);
                }
        }
    }
}