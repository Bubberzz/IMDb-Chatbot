using IMDb_Chatbot.Bots;
using IMDb_Chatbot.Tests.Common;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IMDb_Chatbot.Tests.Bots
{
    public class DialogAndWelcomeBotTests
    {
        [Fact]
        public async Task ReturnsWelcomeCardOnConversationUpdate()
        {
            // Arrange
            var mockRootDialog = SimpleMockFactory.CreateMockDialog<Dialog>(null, "mockRootDialog");
            var memoryStorage = new MemoryStorage();
            var sut = new DialogAndWelcomeBot<Dialog>(new ConversationState(memoryStorage), new UserState(memoryStorage), mockRootDialog.Object, null);

            // Create conversationUpdate activity
            var conversationUpdateActivity = new Activity
            {
                Type = ActivityTypes.ConversationUpdate,
                MembersAdded = new List<ChannelAccount>
                {
                    new ChannelAccount { Id = "theUser" },
                },
                Recipient = new ChannelAccount { Id = "theBot" },
            };
            var testAdapter = new TestAdapter(Channels.Test);

            // Act
            // Send the conversation update activity to the bot.
            await testAdapter.ProcessActivityAsync(conversationUpdateActivity, sut.OnTurnAsync, CancellationToken.None);

            // Assert we got the welcome card
            var reply = (IMessageActivity)testAdapter.GetNextReply();
            Assert.Equal(1, reply.Attachments.Count);
            Assert.Equal("application/vnd.microsoft.card.hero", reply.Attachments[0].ContentType);

            // Assert that we started the main dialog.
            var attachment = reply.Attachments[0].Content;
            var getTitle = attachment.GetType().GetProperty("Title");
            if (getTitle is not null)
            {
                var title = (getTitle.GetValue(attachment, null)) as string;

                Assert.Equal("Welcome to IMDb bot!", title);
            }

            var getText = attachment.GetType().GetProperty("Text");
            if (getText is not null)
            {
                var text = (getText.GetValue(attachment, null)) as string;

                Assert.Equal("Search for a film or actor or type 'Options' for more options", text);
            }
        }
    }
}
