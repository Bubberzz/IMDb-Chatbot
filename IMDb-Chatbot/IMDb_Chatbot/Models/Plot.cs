using System.Collections.Generic;
using Newtonsoft.Json;

namespace IMDb_Chatbot.Models
{
    public class Plot
    {
        public class Image
        {
            public int height { get; set; }
            public string id { get; set; }
            public string url { get; set; }
            public int width { get; set; }
        }

        public class Base
        {
            [JsonProperty("@type")] public string Type { get; set; }

            public string id { get; set; }
            public Image image { get; set; }
            public string title { get; set; }
            public string titleType { get; set; }
            public int year { get; set; }
        }

        public class FilmPlot
        {
            public string author { get; set; }
            public string id { get; set; }
            public string text { get; set; }
        }

        public class PlotRoot
        {
            public string id { get; set; }
            public Base @base { get; set; }
            public List<FilmPlot> plots { get; set; }
        }
    }
}