using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IMDb_Chatbot.Interfaces;
using IMDb_Chatbot.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace IMDb_Chatbot.Services
{
    public class ImdbService : IImdbService
    {
        private readonly IConfiguration _configuration;

        public ImdbService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ImdbSearch.Root> GetSearchResult(string search)
        {
            var client = new HttpClient();
            var request = CreateRequest("https://imdb8.p.rapidapi.com/title/find?q=" + search);
            string body;
            try
            {
                using var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                body = await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return new ImdbSearch.Root();
            }

            var deserializedBody = JsonConvert.DeserializeObject<ImdbSearch.Root>(body);

            // If null, return to calling function where it will be handled
            if (deserializedBody.results is null) return deserializedBody;

            // We only want 5 or less results, api typically brings back 20
            if (deserializedBody.results.Count < 5) return deserializedBody;

            var reducedResultsList = new ImdbSearch.Root
            {
                results = new List<ImdbSearch.Result>
                {
                    new(),
                    new(),
                    new(),
                    new(),
                    new()
                }
            };
            for (var i = 0; i < 5; i++) reducedResultsList.results[i] = deserializedBody.results[i];
            return reducedResultsList;
        }

        public async Task<Rating.RatingRoot> GetRating(string id)
        {
            var client = new HttpClient();
            var request = CreateRequest("https://imdb8.p.rapidapi.com/title/get-ratings?tconst=" + id);
            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var deserializedBody = JsonConvert.DeserializeObject<Rating.RatingRoot>(body);
            return deserializedBody;
        }

        public async Task<Plot.PlotRoot> GetPlot(string id)
        {
            var client = new HttpClient();
            var request = CreateRequest("https://imdb8.p.rapidapi.com/title/get-plots?tconst=" + id);
            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var deserializedBody = JsonConvert.DeserializeObject<Plot.PlotRoot>(body);
            return deserializedBody;
        }

        public async Task<List<string>> GetGenre(string id)
        {
            var client = new HttpClient();
            var request = CreateRequest("https://imdb8.p.rapidapi.com/title/get-genres?tconst=" + id);
            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var deserializedBody = JsonConvert.DeserializeObject<List<string>>(body);
            return deserializedBody;
        }

        public async Task<Bio.BioRoot> GetBio(string id)
        {
            var client = new HttpClient();
            var request = CreateRequest("https://imdb8.p.rapidapi.com/actors/get-bio?nconst=" + id);
            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var deserializedBody = JsonConvert.DeserializeObject<Bio.BioRoot>(body);
            return deserializedBody;
        }

        public async Task<AutoComplete.Root> AutoComplete(string search)
        {
            try
            {
                var client = new HttpClient();
                var request = CreateRequest("https://imdb8.p.rapidapi.com/auto-complete?q=" + search);
                using var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var deserializedBody = JsonConvert.DeserializeObject<AutoComplete.Root>(body);
                return deserializedBody;
            }
            catch
            {
                return new AutoComplete.Root();
            }
        }

        public async Task<List<TopRatedMovies.Root>> GetTopRatedMovies()
        {
            var client = new HttpClient();
            var request = CreateRequest("https://imdb8.p.rapidapi.com/title/get-top-rated-movies");
            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var deserializedBody = JsonConvert.DeserializeObject<List<TopRatedMovies.Root>>(body);
            return deserializedBody;
        }

        public async Task<List<TopRatedActors.Root>> GetTopRatedActors()
        {
            var client = new HttpClient();
            var request =
                CreateRequest(
                    "https://imdb8.p.rapidapi.com/actors/list-most-popular-celebs?homeCountry=US&currentCountry=US&purchaseCountry=US");
            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var deserializedBody = JsonConvert.DeserializeObject<List<string>>(body);
            var result = new List<TopRatedActors.Root>();
            var rating = 0;
            foreach (var item in deserializedBody)
            {
                rating += 1;
                result.Add(new TopRatedActors.Root {id = item, chartRating = rating});
            }

            return result;
        }

        public async Task<List<string>> ComingSoon()
        {
            var client = new HttpClient();
            var request =
                CreateRequest(
                    "https://imdb8.p.rapidapi.com/title/get-coming-soon-movies?homeCountry=US&purchaseCountry=US&currentCountry=US");
            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var deserializedBody = JsonConvert.DeserializeObject<List<string>>(body);
            return deserializedBody;
        }

        private HttpRequestMessage CreateRequest(string uri)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri),
                Headers =
                {
                    {"x-rapidapi-key", _configuration["APIKey"]},
                    {"x-rapidapi-host", _configuration["APIHost"]}
                }
            };
            return request;
        }
    }
}