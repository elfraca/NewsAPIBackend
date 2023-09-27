using ItemEntity = Domain.Data.Item;
using Domain.Helper;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using Domain.Data;
using System.Reflection.Metadata.Ecma335;
using System.Collections.Generic;
using System.Collections.Concurrent;

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

        public async Task<List<ItemEntity>> GetNewestStoriesAsync(int page = 1, int pageSize = 10)
        {
            List<ItemEntity> itemsListCache;
            List<int> newsIdsList;

            itemsListCache = _memoryCache.Get<List<ItemEntity>>("topitems");
            newsIdsList = _memoryCache.Get<List<int>>("topids");

            if (itemsListCache is null)
            {
                ConcurrentBag<ItemEntity> bag = new ConcurrentBag<ItemEntity>();
                var response = await _httpClient.GetAsync($"topstories.json");
                var content = await response.Content.ReadAsStringAsync();
                newsIdsList = JsonConvert.DeserializeObject<List<int>>(content);
                _memoryCache.Set("topids", newsIdsList);

                itemsListCache = new List<ItemEntity>();

                var optionsParallel = new ParallelOptions { MaxDegreeOfParallelism = 8 };
                await Parallel.ForEachAsync(newsIdsList, optionsParallel, async (news, token) =>
                {
                    ItemEntity item = await GetItemDetailAsync( news );
                    bag.Add(item);
                });

                itemsListCache = bag.ToList();

                _memoryCache.Set("topitems", itemsListCache);
            }
            else
            {
                ConcurrentBag<ItemEntity> bag = new ConcurrentBag<ItemEntity>();
                var response = await _httpClient.GetAsync($"topstories.json");
                var content = await response.Content.ReadAsStringAsync();
                List<int> topNewsItems = JsonConvert.DeserializeObject<List<int>>( content );
                newsIdsList = _memoryCache.Get<List<int>>("topids");
                if (!(newsIdsList.Count == topNewsItems.Count && newsIdsList.All(topNewsItems.Contains)))
                {
                    var optionsParallel = new ParallelOptions { MaxDegreeOfParallelism = 4 };
                    await Parallel.ForEachAsync(itemsListCache, optionsParallel, async (item, token) =>
                    {
                        if (!topNewsItems.Contains(item.Id))
                        {
                            bag.Add(item);
                        }
                    });
                    if (bag.Count != 0)
                    {
                        foreach (var item in bag)
                        {
                            itemsListCache.Remove(item);
                        }
                    }
                    

                    bag = new ConcurrentBag<ItemEntity>();
                    await Parallel.ForEachAsync(topNewsItems, optionsParallel, async (item, token) =>
                    {
                        if (!itemsListCache.Any(x => x.Id == item))
                        {
                            ItemEntity newitem = await GetItemDetailAsync(item);
                            bag.Add(newitem);
                        }
                    });
                    if (bag.Count != 0)
                    {
                        foreach (var item in bag)
                        {
                            itemsListCache.Add(item);
                        }
                    }
                    
                    _memoryCache.Set("topitems", itemsListCache);
                }
                
            }

            var totalCount = itemsListCache.Count;
            var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            var itemsPerPages = itemsListCache
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return itemsPerPages;
        }

    }
}
