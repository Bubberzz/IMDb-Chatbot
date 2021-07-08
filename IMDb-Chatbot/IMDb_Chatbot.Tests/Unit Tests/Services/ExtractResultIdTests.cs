using System.Collections.Generic;
using IMDb_Chatbot.Models;
using IMDb_Chatbot.Services;
using Xunit;

namespace IMDb_Chatbot.Tests.Unit_Tests.Services
{
    public class ExtractResultIdTests
    {
        [Theory]
        [InlineData("/title/tt0137523/")]
        public void WhenGetIdIsCalled_ForImdbSearchDialog_ThenReturnId(string value)
        {
            // Arrange
            var searchResult = new ImdbSearch.Root()
            {
                results = new List<ImdbSearch.Result>()
                {
                    new()
                    {
                        id = value
                    }
                }
            };

            // Act
            var result = ExtractResultId.GetId(searchResult);
            const string expected = "tt0137523";

            // Assert
            Assert.NotNull(result);
            Assert.Equal(9, result.Length);
            Assert.Equal(expected, result);
        }


        [Theory]
        [InlineData("/title/tt0137523/")]
        public void WhenGetIdIsCalled_ForTopRatedMoviesDialog_ThenReturnId(string value)
        {
            // Arrange
            var searchResult = new List<TopRatedMovies.Root>()
            {
                new TopRatedMovies.Root()
                {
                    id = value
                }
            };

            // Act
            var result = ExtractResultId.GetId(searchResult, 0);
            const string expected = "tt0137523";

            // Assert
            Assert.NotNull(result);
            Assert.Equal(9, result.Length);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("/name/nm2088803/")]
        public void WhenGetIdIsCalled_ForTopRatedActorsDialog_ThenReturnId(string value)
        {
            // Arrange
            var searchResult = new List<TopRatedActors.Root>()
            {
                new TopRatedActors.Root()
                {
                    id = value
                }
            };

            // Act
            var result = ExtractResultId.GetId(searchResult, 0);
            const string expected = "nm2088803";

            // Assert
            Assert.NotNull(result);
            Assert.Equal(9, result.Length);
            Assert.Equal(expected, result);
        }
    }
}