using System.Collections.Generic;
using IMDb_Chatbot.Models;

namespace IMDb_Chatbot.Services
{
    public class ExtractResultId
    {
        public static string GetId(ImdbSearch.Root searchResult, int i = 0)
        {
            var extractId = searchResult.results[i].id.Split("/");
            var resultId = extractId[2];
            return resultId;
        }

        public static string GetId(List<TopRatedMovies.Root> searchResult, int i)
        {
            var extractId = searchResult[i].id.Split("/");
            var resultId = extractId[2];
            return resultId;
        }

        public static string GetId(List<TopRatedActors.Root> searchResult, int i)
        {
            var extractId = searchResult[i].id.Split("/");
            var resultId = extractId[2];
            return resultId;
        }
    }
}