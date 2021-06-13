using System.Collections.Generic;
using IMDb_Chatbot.Models;

namespace IMDb_Chatbot.Interfaces
{
    public interface IImdbResult
    {
        public ImdbSearch.Root ImdbSearchResult { get; set; }
    }
}