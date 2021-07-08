using System;
using IMDb_Chatbot.Interfaces;

namespace IMDb_Chatbot.Services
{
    public class GenerateFilmId : IGenerateFilmId
    {
        public string FilmId()
        {
            var random = new Random();
            var resultId = "tt";
            for (var i = 0; i < 7; i++)
                resultId = string.Concat(resultId, random.Next(10).ToString());
            return resultId;
        }
    }
}