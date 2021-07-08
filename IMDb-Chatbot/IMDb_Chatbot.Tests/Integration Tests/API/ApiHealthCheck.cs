using System;
using System.Net.Http;
using System.Threading.Tasks;
using IMDb_Chatbot.Tests.Common;
using Xunit;

namespace IMDb_Chatbot.Tests.Integration_Tests.API
{
    public class ApiHealthCheck : BotTestBase
    {
        [Fact]
        public async Task GivenGetSearchResult_WhenCalled_ThenReturnSuccess()
        {
            // Arrange
            var client = new HttpClient();
            var request = CreateRequest("https://imdb8.p.rapidapi.com/title/find?q=fight%20club");

            // Act
            using var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(body);
        }

        [Fact]
        public async Task GivenGetRating_WhenCalled_ThenReturnSuccess()
        {
            // Arrange
            var client = new HttpClient();
            var request = CreateRequest("https://imdb8.p.rapidapi.com/title/get-ratings?tconst=tt0944947");

            // Act
            using var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(body);
        }

        [Fact]
        public async Task GivenGetPlot_WhenCalled_ThenReturnSuccess()
        {
            // Arrange
            var client = new HttpClient();
            var request = CreateRequest("https://imdb8.p.rapidapi.com/title/get-plots?tconst=tt0944947");

            // Act
            using var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(body);
        }

        [Fact]
        public async Task GivenGetGenre_WhenCalled_ThenReturnSuccess()
        {
            // Arrange
            var client = new HttpClient();
            var request = CreateRequest("https://imdb8.p.rapidapi.com/title/get-genres?tconst=tt0944947");

            // Act
            using var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(body);
        }

        [Fact]
        public async Task GivenGetBio_WhenCalled_ThenReturnSuccess()
        {
            // Arrange
            var client = new HttpClient();
            var request = CreateRequest("https://imdb8.p.rapidapi.com/actors/get-bio?nconst=nm0001667");

            // Act
            using var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(body);
        }

        [Fact]
        public async Task GivenAutoComplete_WhenCalled_ThenReturnSuccess()
        {
            // Arrange
            var client = new HttpClient();
            var request = CreateRequest("https://imdb8.p.rapidapi.com/auto-complete?q=game%20of%20thr");

            // Act
            using var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(body);
        }

        [Fact]
        public async Task GivenGetTopRatedMovies_WhenCalled_ThenReturnSuccess()
        {
            // Arrange
            var client = new HttpClient();
            var request = CreateRequest("https://imdb8.p.rapidapi.com/title/get-top-rated-movies");

            // Act
            using var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(body);
        }

        [Fact]
        public async Task GivenGetTopRatedActors_WhenCalled_ThenReturnSuccess()
        {
            // Arrange
            var client = new HttpClient();
            var request = CreateRequest("https://imdb8.p.rapidapi.com/actors/list-most-popular-celebs?homeCountry=US&currentCountry=US&purchaseCountry=US");

            // Act
            using var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(body);
        }

        [Fact]
        public async Task GivenComingSoon_WhenCalled_ThenReturnSuccess()
        {
            // Arrange
            var client = new HttpClient();
            var request = CreateRequest("https://imdb8.p.rapidapi.com/title/get-coming-soon-movies?homeCountry=US&purchaseCountry=US&currentCountry=US");

            // Act
            using var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(body);
        }

        private HttpRequestMessage CreateRequest(string uri)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri),
                Headers =
                {
                    {"x-rapidapi-key", Configuration["APIKey"]},
                    {"x-rapidapi-host", Configuration["APIHost"]},
                },
            };
            return request;
        }
    }
}