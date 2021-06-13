using System.Collections.Generic;
using System.Threading.Tasks;
using CardsBot.Models;
using IMDb_Chatbot.Models;

namespace IMDb_Chatbot.Interfaces
{
    public interface IImdbService
    {
        public Task<ImdbSearch.Root> GetSearchResult(string search);
        public Task<Rating.RatingRoot> GetRating(string id);
        public Task<Plot.PlotRoot> GetPlot(string id);
        public Task<List<string>> GetGenre(string id);
        public Task<Bio.BioRoot> GetBio(string filmId);
        public Task<AutoComplete.Root> AutoComplete(string filmId);
        public Task<List<TopRatedMovies.Root>> GetTopRatedMovies();
        public Task<List<TopRatedActors.Root>> GetTopRatedActors();
        public Task<List<string>> ComingSoon();
    }
}