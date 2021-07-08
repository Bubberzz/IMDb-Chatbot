using System.Collections.Generic;
using IMDb_Chatbot.Models;

namespace IMDb_Chatbot.Tests.Integration_Tests.Dialogs.TestData
{
    public class TestCases
    {
        public List<TopRatedMovies.Root> TopRatedMovies { get; set; }
        public List<TopRatedActors.Root> TopRatedActors { get; set; }
        public AutoComplete.Root AutoCompleteMovie { get; set; }
        public AutoComplete.Root AutoCompleteActor { get; set; }
        public List<string> ComingSoonMovies { get; set; }
        public ImdbSearch.Root MovieSearchResult { get; set; }
        public Plot.PlotRoot Plot { get; set; }
        public Rating.RatingRoot Rating { get; set; }
        public List<string> Genre { get; set; }
        public ImdbSearch.Root ActorSearchResult { get; set; }
        public Bio.BioRoot Bio { get; set; }
    }
}