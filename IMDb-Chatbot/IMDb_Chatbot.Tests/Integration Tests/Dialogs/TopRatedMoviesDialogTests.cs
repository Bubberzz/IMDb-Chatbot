using System.Collections.Generic;
using System.Threading.Tasks;
using IMDb_Chatbot.Dialogs;
using IMDb_Chatbot.Interfaces;
using IMDb_Chatbot.Tests.Common;
using IMDb_Chatbot.Tests.Integration_Tests.Dialogs.TestData;
using Microsoft.Bot.Builder.Testing;
using Microsoft.Bot.Builder.Testing.XUnit;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace IMDb_Chatbot.Tests.Integration_Tests.Dialogs
{
    public class TopRatedMoviesDialogTests : BotTestBase
    {
        private readonly ComingSoonMoviesDialog _mockComingSoonMoviesDialog;
        private readonly ImdbSearchDialog _mockImdbSearchDialog;
        private readonly IImdbService _mockImdbService;
        private readonly Mock<ILogger<MainDialog>> _mockLogger;
        private readonly MovieRouletteDialog _mockMovieRouletteDialog;
        private readonly TopRatedActorsDialog _mockTopRatedActorsDialog;

        private readonly TopRatedMoviesDialog _mockTopRatedMoviesDialog;

        public TopRatedMoviesDialogTests()
        {
            _mockTopRatedMoviesDialog = new TopRatedMoviesDialog(_mockImdbService);
            _mockTopRatedActorsDialog = new TopRatedActorsDialog(_mockImdbService);
            _mockComingSoonMoviesDialog = new ComingSoonMoviesDialog(_mockImdbService);
            _mockMovieRouletteDialog = new MovieRouletteDialog(_mockImdbService);
            _mockImdbSearchDialog = new ImdbSearchDialog(_mockImdbService);
            _mockImdbService = new Mock<IImdbService>().Object;
            _mockLogger = new Mock<ILogger<MainDialog>>();
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.IMDbTestData), MemberType = typeof(TestDataGenerator))]
        public async Task GivenTopRatedMoviesDialog_WhenUserSelectsOptions_ThenReturnHeroCard(TestDataObject testData)
        {
            // Arrange
            var data = testData.GetObject<TestCases>();
            var mock = new Mock<IImdbService>();
            mock.Setup(x => x.GetTopRatedMovies())
                .Returns(Task.FromResult(data.TopRatedMovies));
            mock.Setup(x => x.AutoComplete(It.IsAny<string>()))
                .Returns(Task.FromResult(data.AutoCompleteMovie));

            var heroCard = new HeroCard
            {
                Text = ExpectedTopRatedMoviesContent,
                Buttons = new List<CardAction>
                {
                    new(ActionTypes.MessageBack, "More Results",
                        value: "Show More: Top rated movies")
                }
            };

            // Act
            var sut = new MainDialog(_mockTopRatedMoviesDialog, _mockTopRatedActorsDialog, _mockComingSoonMoviesDialog,
                _mockMovieRouletteDialog,
                _mockImdbSearchDialog, _mockLogger.Object, mock.Object);
            var testClient = new DialogTestClient(Channels.Msteams, sut);

            // Assert
            var reply = await testClient.SendActivityAsync<IMessageActivity>("Options");
            reply = await testClient.SendActivityAsync<IMessageActivity>("Top rated movies");
            Assert.Equal("Loading..", reply.Text);

            reply = testClient.GetNextReply<IMessageActivity>();
            Assert.NotNull(reply.Attachments[0]);
            Assert.Equal("list", reply.AttachmentLayout);
            Assert.Equal("Microsoft.Bot.Schema.HeroCard", reply.Attachments[0].Content.ToString());

            var expected = JsonConvert.SerializeObject(heroCard);
            var actual = JsonConvert.SerializeObject(reply.Attachments[0].Content);
            Assert.Equal(expected, actual);

            reply = testClient.GetNextReply<IMessageActivity>();
            Assert.Equal("Type anything to do another search or type 'Options'", reply.Text);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.IMDbTestData), MemberType = typeof(TestDataGenerator))]
        public async Task GivenTopRatedMoviesDialog_WhenUserSelectsMoreResults_ThenReturnHeroCard(
            TestDataObject testData)
        {
            // Arrange
            var data = testData.GetObject<TestCases>();
            var mock = new Mock<IImdbService>();
            mock.Setup(x => x.GetTopRatedMovies())
                .Returns(Task.FromResult(data.TopRatedMovies));
            mock.Setup(x => x.AutoComplete(It.IsAny<string>()))
                .Returns(Task.FromResult(data.AutoCompleteMovie));

            var heroCard = new HeroCard
            {
                Text = ExpectedTopRatedMoviesContent,
                Buttons = new List<CardAction>
                {
                    new(ActionTypes.MessageBack, "More Results",
                        value: "Show More: Top rated movies")
                }
            };

            // Act
            var sut = new MainDialog(_mockTopRatedMoviesDialog, _mockTopRatedActorsDialog, _mockComingSoonMoviesDialog,
                _mockMovieRouletteDialog,
                _mockImdbSearchDialog, _mockLogger.Object, mock.Object);
            var testClient = new DialogTestClient(Channels.Msteams, sut);

            // Assert
            var reply = await testClient.SendActivityAsync<IMessageActivity>("Options");
            reply = await testClient.SendActivityAsync<IMessageActivity>("Top rated movies");
            Assert.Equal("Loading..", reply.Text);

            reply = testClient.GetNextReply<IMessageActivity>();
            var activity = new Activity(value: "Show More: Top rated movies");
            var activityReply = testClient.SendActivityAsync<IActivity>(activity);
            activityReply = testClient.SendActivityAsync<IActivity>(activity);
            activityReply = testClient.SendActivityAsync<IActivity>(activity);

            Assert.NotNull(reply.Attachments[0]);
            Assert.Equal("list", reply.AttachmentLayout);
            Assert.Equal("Microsoft.Bot.Schema.HeroCard", reply.Attachments[0].Content.ToString());

            var expected = JsonConvert.SerializeObject(heroCard);
            var actual = JsonConvert.SerializeObject(reply.Attachments[0].Content);
            Assert.Equal(expected, actual);

            reply = testClient.GetNextReply<IMessageActivity>();
            Assert.Equal("Type anything to do another search or type 'Options'", reply.Text);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.IMDbTestData), MemberType = typeof(TestDataGenerator))]
        public async Task
            GivenTopRatedMoviesDialog_WhenUserSelectsMoreResults_AndThereIsNoMoreResults_ThenReturnFinalCard(
                TestDataObject testData)
        {
            // Arrange
            var data = testData.GetObject<TestCases>();
            var mock = new Mock<IImdbService>();
            mock.Setup(x => x.GetTopRatedMovies())
                .Returns(Task.FromResult(data.TopRatedMovies));
            mock.Setup(x => x.AutoComplete(It.IsAny<string>()))
                .Returns(Task.FromResult(data.AutoCompleteMovie));

            var heroCard = new HeroCard
            {
                Text = ExpectedTopRatedMoviesContentLastResult
            };

            // Act
            var sut = new MainDialog(_mockTopRatedMoviesDialog, _mockTopRatedActorsDialog, _mockComingSoonMoviesDialog,
                _mockMovieRouletteDialog,
                _mockImdbSearchDialog, _mockLogger.Object, mock.Object);
            var testClient = new DialogTestClient(Channels.Msteams, sut);

            // Assert
            var reply = await testClient.SendActivityAsync<IMessageActivity>("Options");
            reply = await testClient.SendActivityAsync<IMessageActivity>("Top rated movies");
            Assert.Equal("Loading..", reply.Text);

            reply = testClient.GetNextReply<IMessageActivity>();
            Assert.NotNull(reply.Attachments[0]);
            Assert.Equal("list", reply.AttachmentLayout);
            Assert.Equal("Microsoft.Bot.Schema.HeroCard", reply.Attachments[0].Content.ToString());

            reply = testClient.GetNextReply<IMessageActivity>();
            Assert.Equal("Type anything to do another search or type 'Options'", reply.Text);

            // Simulate the user clicking More Results button until we get to the last one
            var activity = new Activity(value: "Show More: Top rated movies");
            var activityReply = testClient.SendActivityAsync<IActivity>(activity);
            reply = testClient.GetNextReply<IMessageActivity>();
            reply = testClient.GetNextReply<IMessageActivity>();

            activityReply = testClient.SendActivityAsync<IActivity>(activity);
            reply = testClient.GetNextReply<IMessageActivity>();
            reply = testClient.GetNextReply<IMessageActivity>();

            activityReply = testClient.SendActivityAsync<IActivity>(activity);
            reply = testClient.GetNextReply<IMessageActivity>();
            reply = testClient.GetNextReply<IMessageActivity>();

            activityReply = testClient.SendActivityAsync<IActivity>(activity);
            reply = testClient.GetNextReply<IMessageActivity>();
            reply = testClient.GetNextReply<IMessageActivity>();

            activityReply = testClient.SendActivityAsync<IActivity>(activity);
            reply = testClient.GetNextReply<IMessageActivity>();
            reply = testClient.GetNextReply<IMessageActivity>();

            activityReply = testClient.SendActivityAsync<IActivity>(activity);
            reply = testClient.GetNextReply<IMessageActivity>();
            reply = testClient.GetNextReply<IMessageActivity>();

            activityReply = testClient.SendActivityAsync<IActivity>(activity);
            reply = testClient.GetNextReply<IMessageActivity>();
            reply = testClient.GetNextReply<IMessageActivity>();

            activityReply = testClient.SendActivityAsync<IActivity>(activity);
            reply = testClient.GetNextReply<IMessageActivity>();
            reply = testClient.GetNextReply<IMessageActivity>();

            activityReply = testClient.SendActivityAsync<IActivity>(activity);
            reply = testClient.GetNextReply<IMessageActivity>();

            var expected = JsonConvert.SerializeObject(heroCard);
            var actual = JsonConvert.SerializeObject(reply.Attachments[0].Content);
            Assert.Equal(expected, actual);

            reply = testClient.GetNextReply<IMessageActivity>();
            Assert.Equal("Type anything to do another search or type 'Options'", reply.Text);
        }
    }
}