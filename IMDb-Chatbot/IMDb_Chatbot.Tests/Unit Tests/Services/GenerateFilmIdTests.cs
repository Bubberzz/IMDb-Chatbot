using System.Linq;
using IMDb_Chatbot.Interfaces;
using IMDb_Chatbot.Services;
using Xunit;

namespace IMDb_Chatbot.Tests.Unit_Tests.Services
{
    public class GenerateFilmIdTests
    {
        public IGenerateFilmId GenerateFilmId { get; }

        public GenerateFilmIdTests()
        {
            GenerateFilmId = new GenerateFilmId();
        }

        [Fact]
        public void GivenGenerateFilmId_ThenReturnRandomId()
        {
            // Act
            var filmId = GenerateFilmId.FilmId();
            var filmIdDigits = filmId[2..];

            // Assert
            Assert.Equal(9, filmId.Length);
            Assert.Contains("tt", filmId);
            Assert.True(filmId != null && filmId.Any(char.IsDigit));
            Assert.True(filmIdDigits.All(char.IsDigit));
        }
    }
}