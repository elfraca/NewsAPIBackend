using ItemEntity = Domain.Data.Item;
using Domain.Helper;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using Domain.Data;
using System.Reflection.Metadata.Ecma335;
using System.Collections.Generic;

namespace Domain.Services.Item
{
    
    public class ItemService : IItemService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;

        public ItemService(HttpClient httpClient, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(HttpHelper.Url);
        }


        public async Task<ItemEntity> GetItemDetailAsync(int itemId)
        {
            var response = await _httpClient.GetAsync($"item/{itemId}.json");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var item = JsonConvert.DeserializeObject<ItemEntity>( content );
                return item;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<ItemEntity>> GetNewestStoriesAsync()
        {
            List<ItemEntity> itemsCache = new List<ItemEntity>();
            List<int> newsList = new List<int>();

            itemsCache = _memoryCache.Get<List<ItemEntity>>("topitems");
            newsList = _memoryCache.Get<List<int>>("topids");

            if (itemsCache is null)
            {
                var response = await _httpClient.GetAsync($"topstories.json");
                var content = await response.Content.ReadAsStringAsync();
                newsList = JsonConvert.DeserializeObject<List<int>>(content);
                _memoryCache.Set("topids", newsList);

                itemsCache = new List<ItemEntity>();

                foreach ( var news in newsList ) 
                {
                    ItemEntity item = await GetItemDetailAsync( news );
                    itemsCache.Add(item);
                }

                _memoryCache.Set("topitems", itemsCache);
            }
            else
            {
                var response = await _httpClient.GetAsync($"topstories.json");
                var content = await response.Content.ReadAsStringAsync();
                List<int> topNewsItems = JsonConvert.DeserializeObject<List<int>>( content );
                newsList = _memoryCache.Get<List<int>>("topids");
                if (!(newsList.Count == topNewsItems.Count && newsList.All(topNewsItems.Contains)))
                {
                    foreach (var item in itemsCache)
                    {
                        if (!topNewsItems.Contains(item.Id))
                        {
                            itemsCache.Remove(item);
                        }
                    }

                    foreach (var topitem in topNewsItems)
                    {
                        if (itemsCache.Any(x => x.Id == topitem))
                        {
                            ItemEntity item = await GetItemDetailAsync(topitem);
                            itemsCache.Add(item);
                        }
                    }
                    _memoryCache.Set("topitems", itemsCache);
                }
                
            }

            return itemsCache ;
        }

    }
}
