// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
using Xunit.Abstractions;

namespace IMDb_Chatbot.Tests.Integration_Tests.Dialogs
{
    public class MainDialogTests : BotTestBase
    {
        private readonly TopRatedMoviesDialog _mockTopRatedMoviesDialog;
        private readonly TopRatedActorsDialog _mockTopRatedActorsDialog;
        private readonly ComingSoonMoviesDialog _mockComingSoonMoviesDialog;
        private readonly MovieRouletteDialog _mockMovieRouletteDialog;
        private readonly ImdbSearchDialog _mockImdbSearchDialog;
        private readonly IImdbService _mockImdbService;
        private readonly Mock<ILogger<MainDialog>> _mockLogger;
      
        public MainDialogTests(ITestOutputHelper output)
            : base(output)
        {
            _mockLogger = new Mock<ILogger<MainDialog>>();
            _mockTopRatedMoviesDialog = new TopRatedMoviesDialog(_mockImdbService);
            _mockTopRatedActorsDialog = new TopRatedActorsDialog(_mockImdbService);
            _mockComingSoonMoviesDialog = new ComingSoonMoviesDialog(_mockImdbService);
            _mockMovieRouletteDialog = new MovieRouletteDialog(_mockImdbService);
            _mockImdbSearchDialog = new ImdbSearchDialog(_mockImdbService);
            _mockImdbService = new Mock<IImdbService>().Object;
        }


        [Fact]
        public async Task WhenUserRequestsOptions_ThenReturnOptions()
        {
            // Arrange
            var sut = new MainDialog(_mockTopRatedMoviesDialog, _mockTopRatedActorsDialog, _mockComingSoonMoviesDialog, _mockMovieRouletteDialog, 
                _mockImdbSearchDialog, _mockLogger.Object, _mockImdbService);

            // Act
            var testClient = new DialogTestClient(Channels.Msteams, sut);
            
            // Assert
            var reply = await testClient.SendActivityAsync<IMessageActivity>("Options");
            Assert.Equal("Pick one of the following options\n\n   1. Top rated movies\n   2. Top rated actors\n   3. Coming soon movies\n   4. IMDb Roulette",
                reply.Text);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.IMDbTestData), MemberType = typeof(TestDataGenerator))]
        public async Task GivenMainDialog_WhenUserSearchesForMovie_ThenReturnHeroCardWithResult(TestDataObject testData)
        {
            // Arrange
            var data = testData.GetObject<TestCases>();
            var mock = new Mock<IImdbService>();
            mock.Setup(x => x.GetSearchResult(It.IsAny<string>()))
                .Returns(Task.FromResult(data.MovieSearchResult));
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
                Text = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.\r\n Year: 1994 | Rating: 9.3 | Genre: Drama | Minutes: 142 | Type: movie",
                Images = new List<CardImage> { new CardImage("https://m.media-amazon.com/images/M/MV5BMDFkYTc0MGEtZmNhMC00ZDIzLWFmNTEtODM1ZmRlYWMwMWFmXkEyXkFqcGdeQXVyMTMxODk2OTU@._V1_.jpg") },
                Buttons = new List<CardAction>
                {
                    new(ActionTypes.OpenUrl, "Open IMDb",
                        value: "https://www.imdb.com/title/tt0111161/"),
                    new(ActionTypes.MessageBack, "More Results",
                        value: "Show More: Movie Results")
                },
            };

            // Act
            var sut = new MainDialog(_mockTopRatedMoviesDialog, _mockTopRatedActorsDialog, _mockComingSoonMoviesDialog, _mockMovieRouletteDialog,
                _mockImdbSearchDialog, _mockLogger.Object, mock.Object);
            var testClient = new DialogTestClient(Channels.Msteams, sut);

            // Assert
            var reply = await testClient.SendActivityAsync<IMessageActivity>("The Shawshank Redemption");
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
        public async Task GivenMainDialog_WhenUserSelectsMoreResults_ThenReturnHeroCards(TestDataObject testData)
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

            // Act
            var sut = new MainDialog(_mockTopRatedMoviesDialog, _mockTopRatedActorsDialog, _mockComingSoonMoviesDialog, _mockMovieRouletteDialog,
                _mockImdbSearchDialog, _mockLogger.Object, mock.Object);
            var testClient = new DialogTestClient(Channels.Msteams, sut);

            // Assert
            var reply = await testClient.SendActivityAsync<IMessageActivity>("The Shawshank Redemption");
            reply = testClient.GetNextReply<IMessageActivity>();
            Assert.Equal("Type anything to do another search or type 'Options'", reply.Text);

            var activity = new Activity(value: "Show More: Movie Results");
            var activityReply = testClient.SendActivityAsync<IActivity>(activity);
            reply = testClient.GetNextReply<IMessageActivity>();

            Assert.NotNull(reply.Attachments[0]);
            Assert.Equal("carousel", reply.AttachmentLayout);
            Assert.Equal("Microsoft.Bot.Schema.HeroCard", reply.Attachments[0].Content.ToString());

            var actual = JsonConvert.SerializeObject(reply.Attachments);
            Assert.Equal(ExpectedMoreResults, actual);

            reply = testClient.GetNextReply<IMessageActivity>();
            Assert.Equal("Type anything to do another search or type 'Options'", reply.Text);
        }

        [Theory]
        [MemberData(nameof(TestDataGenerator.IMDbTestData), MemberType = typeof(TestDataGenerator))]
        public async Task GivenMainDialog_WhenUserSearchesForActor_ThenReturnHeroCardWithResult(TestDataObject testData)
        {
            // Arrange
            var data = testData.GetObject<TestCases>();
            var mock = new Mock<IImdbService>();
            mock.Setup(x => x.GetSearchResult(It.IsAny<string>()))
                .Returns(Task.FromResult(data.ActorSearchResult));
            mock.Setup(x => x.AutoComplete(It.IsAny<string>()))
                .Returns(Task.FromResult(data.AutoCompleteActor));
            mock.Setup(x => x.GetBio(It.IsAny<string>()))
                .Returns(Task.FromResult(data.Bio));

            var heroCard = new HeroCard
            {
                Title = "Sophia Di Martino",
                Subtitle = "IMDb rank: 2",
                Text = " was born on 15 January 1983 in Nottingham, Nottinghamshire, England, UK\r\nSophia Di Martino was born on November 15, 1983 in Nottingham, Nottinghamshire, England. She is an actress and director, known for Flowers (2016), Loki (2021) and Yesterday (2019).\r\n Known for: Flowers, Loki, Yesterday, Into the Badlands\r\n",
                Images = new List<CardImage> { new("https://m.media-amazon.com/images/M/MV5BYjk4YmMyOWYtMzlhZS00Yjg4LTk0NjQtNDdmMmViMzk3MDZhXkEyXkFqcGdeQXVyMjQwMDg0Ng@@._V1_.jpg") },
                Buttons = new List<CardAction>
                {
                    new(ActionTypes.OpenUrl, "Open IMDb",
                        value: "https://www.imdb.com/name/nm1620545/")
                },
            };

            // Act
            var sut = new MainDialog(_mockTopRatedMoviesDialog, _mockTopRatedActorsDialog, _mockComingSoonMoviesDialog, _mockMovieRouletteDialog,
                _mockImdbSearchDialog, _mockLogger.Object, mock.Object);
            var testClient = new DialogTestClient(Channels.Msteams, sut);

            // Assert
            var reply = await testClient.SendActivityAsync<IMessageActivity>("Will Smith");
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
