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
    public class MovieRouletteDialogTests : BotTestBase
    {
        private readonly ComingSoonMoviesDialog _mockComingSoonMoviesDialog;
        private readonly ImdbSearchDialog _mockImdbSearchDialog;
        private readonly IImdbService _mockImdbService;
        private readonly Mock<ILogger<MainDialog>> _mockLogger;
        private readonly MovieRouletteDialog _mockMovieRouletteDialog;
        private readonly TopRatedActorsDialog _mockTopRatedActorsDialog;
        private readonly TopRatedMoviesDialog _mockTopRatedMoviesDialog;

        public MovieRouletteDialogTests()
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
        public async Task GivenMovieRouletteDialog_WhenUserSelectsOptions_ThenReturnHeroCard(TestDataObject testData)
        {
            // Arrange
            var data = testData.GetObject<TestCases>();
            var mock = new Mock<IImdbService>();
            mock.Setup(x => x.GetSearchResult(It.IsAny<string>()))
                .Returns(Task.FromResult(data.MovieSearchResult));
            mock.Setup(x => x.AutoComplete(It.IsAny<string>()))
                .Returns(Task.FromResult(data.AutoCompleteMovie));
            mock.Setup(x => x.GetPlot(It.IsAny<string>()))
                .Returns(Task.FromResult(data.Plot));
            mock.Setup(x => x.GetRating(It.IsAny<string>()))
                .Returns(Task.FromResult(data.Rating));
            mock.Setup(x => x.GetGenre(It.IsAny<string>()))
                .Returns(Task.FromResult(data.Genre));

            var heroCard = new HeroCard
            {
                Title = "The Shawshank Redemption",
                Subtitle = "Tim Robbins, Morgan Freeman, Bob Gunton",
                Text =
                    "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.\r\n Year: 1994 | Rating: 9.3 | Genre: Drama | Minutes: 142 | Type: movie",
                Images = new List<CardImage>
                {
                    new(
                        "https://m.media-amazon.com/images/M/MV5BMDFkYTc0MGEtZmNhMC00ZDIzLWFmNTEtODM1ZmRlYWMwMWFmXkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_.jpg")
                },
                Buttons = new List<CardAction>
                {
                    new(ActionTypes.OpenUrl, "Open IMDb",
                        value: "https://www.imdb.com/title/tt0111161/"),
                    new(ActionTypes.MessageBack, "Try Again?",
                        value: "IMDb Roulette")
                }
            };

            // Act
            var sut = new MainDialog(_mockTopRatedMoviesDialog, _mockTopRatedActorsDialog, _mockComingSoonMoviesDialog,
                _mockMovieRouletteDialog,
                _mockImdbSearchDialog, _mockLogger.Object, mock.Object);
            var testClient = new DialogTestClient(Channels.Msteams, sut);

            // Assert
            var reply = await testClient.SendActivityAsync<IMessageActivity>("Options");
            reply = await testClient.SendActivityAsync<IMessageActivity>("IMDb Roulette");
            Assert.Equal("Finding something to watch.. this may take a moment", reply.Text);

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
        public async Task GivenMovieRouletteDialog_WhenUserSelectsTryAgain_ThenReturnHeroCard(TestDataObject testData)
        {
            // Arrange
            var data = testData.GetObject<TestCases>();
            var mock = new Mock<IImdbService>();
            mock.Setup(x => x.GetSearchResult("The%20Shawshank%20Redemption"))
                .Returns(Task.FromResult(data.MovieSearchResult));
            mock.Setup(x => x.AutoComplete(It.IsAny<string>()))
                .Returns(Task.FromResult(data.AutoCompleteMovie));
            mock.Setup(x => x.GetPlot(It.IsAny<string>()))
                .Returns(Task.FromResult(data.Plot));
            mock.Setup(x => x.GetRating(It.IsAny<string>()))
                .Returns(Task.FromResult(data.Rating));
            mock.Setup(x => x.GetGenre(It.IsAny<string>()))
                .Returns(Task.FromResult(data.Genre));

            var heroCard = new HeroCard
            {
                Title = "The Shawshank Redemption",
                Subtitle = "Tim Robbins, Morgan Freeman, Bob Gunton",
                Text =
                    "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.\r\n Year: 1994 | Rating: 9.3 | Genre: Drama | Minutes: 142 | Type: movie",
                Images = new List<CardImage>
                {
                    new(
                        "https://m.media-amazon.com/images/M/MV5BMDFkYTc0MGEtZmNhMC00ZDIzLWFmNTEtODM1ZmRlYWMwMWFmXkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_.jpg")
                },
                Buttons = new List<CardAction>
                {
                    new(ActionTypes.OpenUrl, "Open IMDb",
                        value: "https://www.imdb.com/title/tt0111161/"),
                    new(ActionTypes.MessageBack, "Try Again?",
                        value: "IMDb Roulette")
                }
            };

            // Act
            var sut = new MainDialog(_mockTopRatedMoviesDialog, _mockTopRatedActorsDialog, _mockComingSoonMoviesDialog,
                _mockMovieRouletteDialog,
                _mockImdbSearchDialog, _mockLogger.Object, mock.Object);
            var testClient = new DialogTestClient(Channels.Msteams, sut);

            // Assert
            var reply = await testClient.SendActivityAsync<IMessageActivity>("Options");
            reply = await testClient.SendActivityAsync<IMessageActivity>("IMDb Roulette");
            Assert.Equal("Finding something to watch.. this may take a moment", reply.Text);

            reply = testClient.GetNextReply<IMessageActivity>();
            var activity = new Activity(value: "IMDb Roulette");
            var activityReply = testClient.SendActivityAsync<IActivity>(activity);
            reply = testClient.GetNextReply<IMessageActivity>();
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
    }
}