using System.Collections.Generic;
using Newtonsoft.Json;

namespace IMDb_Chatbot.Models
{
    public class ImdbSearch
    {
        public class Meta
        {
            public string operation { get; set; }
            public string requestId { get; set; }
            public double serviceTimeMs { get; set; }
        }

        public class Image
        {
            public int height { get; set; }
            public string id { get; set; }
            public string url { get; set; }
            public int width { get; set; }
        }

        public class Role
        {
            public string character { get; set; }
            public string characterId { get; set; }
        }

        public class Principal
        {
            public string id { get; set; }
            public string legacyNameText { get; set; }
            public string name { get; set; }
            public int billing { get; set; }
            public string category { get; set; }
            public List<string> characters { get; set; }
            public List<Role> roles { get; set; }
            public string disambiguation { get; set; }
            public string @as { get; set; }
            public int? endYear { get; set; }
            public int? episodeCount { get; set; }
            public int? startYear { get; set; }
            public List<string> attr { get; set; }
        }

        public class Crew
        {
            public string category { get; set; }
        }

        public class Summary
        {
            public string category { get; set; }
            public string displayYear { get; set; }
        }

        public class KnownFor
        {
            public List<Crew> crew { get; set; }
            public Summary summary { get; set; }
            public string id { get; set; }
            public string title { get; set; }
            public string titleType { get; set; }
            public int year { get; set; }
        }

        public class ParentTitle
        {
            public string disambiguation { get; set; }
            public string id { get; set; }
            public Image image { get; set; }
            public string title { get; set; }
            public string titleType { get; set; }
            public int year { get; set; }
        }

        public class Result
        {
            public string id { get; set; }
            public string genre { get; set; }
            public string rating { get; set; }
            public string plot { get; set; }
            public int actorRank { get; set; }
            public string bio { get; set; }
            public string born { get; set; }
            public string realName { get; set; }
            public Image image { get; set; }
            public string runningTimeInMinutes { get; set; }
            public string title { get; set; }
            public string titleType { get; set; }
            public string year { get; set; }
            public List<Principal> principals { get; set; }
            public string nextEpisode { get; set; }
            public int? numberOfEpisodes { get; set; }
            public int? seriesStartYear { get; set; }
            public string legacyNameText { get; set; }
            public string name { get; set; }
            public List<KnownFor> knownFor { get; set; }
            public string disambiguation { get; set; }
            public int? episode { get; set; }
            public int? season { get; set; }
            public ParentTitle parentTitle { get; set; }
            public string previousEpisode { get; set; }
            public int? seriesEndYear { get; set; }
        }

        public class Root
        {
            [JsonProperty("@meta")]
            public Meta Meta { get; set; }

            [JsonProperty("@type")]
            public string Type { get; set; }
            public string query { get; set; }
            public List<Result> results { get; set; }
            public List<string> types { get; set; }
        }


    }
}
