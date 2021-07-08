using System.Threading.Tasks;
using IMDb_Chatbot.Interfaces;
using IMDb_Chatbot.Models;

namespace IMDb_Chatbot.Services
{
    public class GetFilmDetails : IGetFilmDetails
    {
        private static IImdbService _imdbService;

        public GetFilmDetails(IImdbService imdbService)
        {
            _imdbService = imdbService;
        }

        public async Task GetDetails(ImdbSearch.Root response, string resultId, int i = 0)
        {
            // get plot and assign to film object
            var plot = await _imdbService.GetPlot(resultId);
            response.results[i].plot = plot.plots.Count != 0 ? plot.plots[0].text : "";

            // get rating and assign to film object
            var rating = await _imdbService.GetRating(resultId);
            response.results[i].rating = rating.rating ?? "";

            // get genre and assign to film object
            var genre = await _imdbService.GetGenre(resultId);
            response.results[i].genre = genre != null ? genre[0] : "";
        }
    }
}