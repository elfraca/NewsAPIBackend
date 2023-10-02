using Models;
using ItemEntity = Domain.Data.Item;

namespace Domain.Services.Item
{
    public interface IItemService
    {
        public Task<ItemEntity> GetItemDetailAsync(int itemId);
        public Task<SearchResponse<List<ItemEntity>>> GetNewestStoriesAsync(SearchRequest searchRequest);
    }
}
