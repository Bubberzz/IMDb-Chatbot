using System.Collections.Generic;

namespace CardsBot.Models
{
    public class AutoComplete
    {
        public class I
        {
            public int height { get; set; }
            public string imageUrl { get; set; }
            public int width { get; set; }
        }

        public class V
        {
            public I i { get; set; }
            public string id { get; set; }
            public string l { get; set; }
            public string s { get; set; }
        }

        public class D
        {
            public I i { get; set; }
            public string id { get; set; }
            public string l { get; set; }
            public int rank { get; set; }
            public string s { get; set; }
            public List<V> v { get; set; }
            public int vt { get; set; }
            public string q { get; set; }
            public int? y { get; set; }
        }

        public class Root
        {
            public List<D> d { get; set; }
            public string q { get; set; }
            public int v { get; set; }
        }
    }
}
