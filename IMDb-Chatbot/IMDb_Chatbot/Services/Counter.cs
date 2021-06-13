using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsBot.Models
{
    public static class Counter
    {
        public static int MinCount { get { return _minCount; } set { _minCount = value; } }

        public static int MaxCount { get { return _maxCount; } set { _maxCount = value; } }
        private static int _minCount { get; set; }
        private static int _maxCount { get; set; }
    }
}
