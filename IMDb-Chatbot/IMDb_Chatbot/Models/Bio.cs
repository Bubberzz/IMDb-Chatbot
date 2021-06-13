using System.Collections.Generic;
using Newtonsoft.Json;

namespace CardsBot.Models
{
    public class Bio
    {
        public class Image
        {
            public int height { get; set; }
            public string id { get; set; }
            public string url { get; set; }
            public int width { get; set; }
        }

        public class Spous
        {
            public string attributes { get; set; }
            public bool current { get; set; }
            public string fromDate { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string toDate { get; set; }
        }

        public class MiniBio
        {
            public string author { get; set; }
            public string id { get; set; }
            public string language { get; set; }
            public string text { get; set; }
            public string userId { get; set; }
        }

        public class BioRoot
        {
            [JsonProperty("@type")] public string Type { get; set; }
            public List<string> akas { get; set; }
            public string id { get; set; }
            public Image image { get; set; }
            public string legacyNameText { get; set; }
            public string name { get; set; }
            public string birthDate { get; set; }
            public string birthPlace { get; set; }
            public string gender { get; set; }
            public double heightCentimeters { get; set; }
            public string realName { get; set; }
            public List<Spous> spouses { get; set; }
            public List<string> trademarks { get; set; }
            public List<MiniBio> miniBios { get; set; }
        }


    }
}
