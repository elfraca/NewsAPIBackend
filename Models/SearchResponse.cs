
namespace Models
{
    public class SearchResponse<T>
    {
        public T Data { get; set; } = default!;
        public int totalPages { get; set; }
    }
}
