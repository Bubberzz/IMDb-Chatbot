using Newtonsoft.Json;

namespace IMDb_Chatbot.Models
{
    public class Rating
    {
        public class Histogram
        {
            public int _1 { get; set; }
            public int _2 { get; set; }
            public int _3 { get; set; }
            public int _4 { get; set; }
            public int _5 { get; set; }
            public int _6 { get; set; }
            public int _7 { get; set; }
            public int _8 { get; set; }
            public int _9 { get; set; }
            public int _10 { get; set; }
        }

        public class Males
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class FemalesAged3044
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class MalesAged45
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class Agedunder18
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class Aged1829
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class FemalesAgedunder18
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class IMDbUsers
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class FemalesAged45
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class Females
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class IMDbStaff
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class MalesAgedunder18
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class Aged45
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class Top1000voters
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class FemalesAged1829
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class MalesAged3044
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class MalesAged1829
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class USusers
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class NonUSusers
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class Aged3044
        {
            public double aggregateRating { get; set; }
            public string demographic { get; set; }
            public Histogram histogram { get; set; }
            public int totalRatings { get; set; }
        }

        public class RatingsHistograms
        {
            public Males Males { get; set; }

            [JsonProperty("FemalesAged30-44")] public FemalesAged3044 FemalesAged3044 { get; set; }

            [JsonProperty("MalesAged45+")] public MalesAged45 MalesAged45 { get; set; }

            public Agedunder18 Agedunder18 { get; set; }

            [JsonProperty("Aged18-29")] public Aged1829 Aged1829 { get; set; }

            public FemalesAgedunder18 FemalesAgedunder18 { get; set; }
            public IMDbUsers IMDbUsers { get; set; }

            [JsonProperty("FemalesAged45+")] public FemalesAged45 FemalesAged45 { get; set; }

            public Females Females { get; set; }
            public IMDbStaff IMDbStaff { get; set; }
            public MalesAgedunder18 MalesAgedunder18 { get; set; }

            [JsonProperty("Aged45+")] public Aged45 Aged45 { get; set; }

            public Top1000voters Top1000voters { get; set; }

            [JsonProperty("FemalesAged18-29")] public FemalesAged1829 FemalesAged1829 { get; set; }

            [JsonProperty("MalesAged30-44")] public MalesAged3044 MalesAged3044 { get; set; }

            [JsonProperty("MalesAged18-29")] public MalesAged1829 MalesAged1829 { get; set; }

            public USusers USusers { get; set; }

            [JsonProperty("Non-USusers")] public NonUSusers NonUSusers { get; set; }

            [JsonProperty("Aged30-44")] public Aged3044 Aged3044 { get; set; }
        }

        public class RatingRoot
        {
            [JsonProperty("@type")] public string Type { get; set; }

            public string id { get; set; }
            public string title { get; set; }
            public string titleType { get; set; }
            public int year { get; set; }
            public int bottomRank { get; set; }
            public bool canRate { get; set; }
            public string rating { get; set; }
            public int ratingCount { get; set; }
            public RatingsHistograms ratingsHistograms { get; set; }
            public int topRank { get; set; }
        }
    }
}