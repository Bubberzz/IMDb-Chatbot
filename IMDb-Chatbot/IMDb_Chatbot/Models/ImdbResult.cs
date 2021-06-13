using System.Collections.Generic;
using CardsBot.Models;
using IMDb_Chatbot.Interfaces;

namespace IMDb_Chatbot.Models
{
    public class ImdbResult : IImdbResult
    {
        public ImdbSearch.Root ImdbSearchResult { get; set; }
        public static List<TopRatedMovies.Root> TopRatedMovies { get; set; }
        public static List<TopRatedActors.Root> TopRatedActors { get; set; }
        public static List<string> ComingSoon { get; set; }
    }
}