using ItemEntity = Domain.Data.Item;

namespace Domain.Services.Item
{
    
    public class ItemService : IItemService
    {
        private readonly HttpClient _httpClient;

        public ItemService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public ItemEntity GetItemDetail(int itemId)
        {
            var item = _httpClient.GetAsync($"item/{itemId}");
            return new ItemEntity();
        }

    }
}
