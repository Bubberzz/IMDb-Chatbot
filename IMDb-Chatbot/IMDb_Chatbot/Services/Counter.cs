namespace IMDb_Chatbot.Services
{
    public static class Counter
    {
        public static int MinCount
        {
            get => _minCount;
            set => _minCount = value;
        }

        public static int MaxCount
        {
            get => _maxCount;
            set => _maxCount = value;
        }

        private static int _minCount { get; set; }
        private static int _maxCount { get; set; }
    }
}