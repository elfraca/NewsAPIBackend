namespace Models
{
    public class SearchRequest
    {
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public string? searchTerm { get; set; }
    }
}