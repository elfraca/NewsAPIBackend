namespace Domain.Data
{
    public class Item
    {
        public int ItemId { get; set; }
        public bool Deleted { get; set; }
        public string Type { get; set; }
        public string By { get; set; }
        public List<int> Kids { get; set; }
        public int Parent { get; set; }
        public int Score { get; set; }
        private static DateTimeOffset Time { get; set; }
        public long TimeUnixTime { get; } = Time.ToUnixTimeSeconds();
        public string Title { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public int Descendants { get; set; }
        public int Poll { get; set; }
    }
}