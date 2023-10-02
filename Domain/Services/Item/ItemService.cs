using ItemEntity = Domain.Data.Item;
using Domain.Helper;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using Models;

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


        public async Task<ItemEntity?> GetItemDetailAsync(int itemId)
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

        public async Task<SearchResponse<List<ItemEntity>>> GetNewestStoriesAsync(SearchRequest searchRequest)
        {
            List<int> newsIdsListCache;
            ConcurrentDictionary<int, ItemEntity> itemsDictionary;
            Dictionary<int, ItemEntity>? returnDictionary;

            returnDictionary = _memoryCache.Get<Dictionary<int, ItemEntity>>("newstories");
            newsIdsListCache = _memoryCache.Get<List<int>>("newscacheid");

            if (returnDictionary is null)
            {
                itemsDictionary = new ConcurrentDictionary<int, ItemEntity>();
                returnDictionary = await LoadCache(itemsDictionary, returnDictionary, newsIdsListCache);
            }
            else
            {
                itemsDictionary = new ConcurrentDictionary<int, ItemEntity>();
                await UpdateCache(itemsDictionary, returnDictionary, newsIdsListCache);

            }
            List<ItemEntity> returnList = returnDictionary.Values.ToList();
            if (!string.IsNullOrEmpty(searchRequest.searchTerm))
            {
                returnList = FilterPhase(searchRequest, returnList);
            }

            SearchResponse<List<ItemEntity>> itemsPerPages = PaginationPhase(searchRequest.page, searchRequest.pageSize, returnList);

            return itemsPerPages;
        }

        private static List<ItemEntity> FilterPhase(SearchRequest searchRequest, List<ItemEntity> listToFilter)
        {
            var trimedSearchTerm = searchRequest.searchTerm.Trim();
            listToFilter = listToFilter.Where(item =>
                (item.Title != null && item.Title.Contains(trimedSearchTerm))
                || (item.Url != null && item.Url.Contains(trimedSearchTerm))
                || (item.By != null && item.By.Contains(trimedSearchTerm))).ToList();
            return listToFilter;
        }

        private static Dictionary<int, ItemEntity> CheckAndRemoveFromDictionary(ConcurrentDictionary<int, ItemEntity> itemsDictionary, List<int> newsIdsListIncome, IEnumerable<int> listToRemove)
        {
            Dictionary<int, ItemEntity> removeDictionary = itemsDictionary.ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (var item in listToRemove)
            {
                if (!newsIdsListIncome.Contains(item))
                {
                    removeDictionary.Remove(item);
                }
            }
            return removeDictionary;
        }

        private async Task CheckAndAddToDictionary(ConcurrentDictionary<int, ItemEntity> itemsDictionary, Dictionary<int, ItemEntity>? returnDictionary, List<int> newsIdsListIncome)
        {
            var optionsParallel = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 2 };
            await Parallel.ForEachAsync(newsIdsListIncome, optionsParallel, async (newItem, token) =>
            {
                if (!returnDictionary.ContainsKey(newItem))
                {
                    ItemEntity item = await GetItemDetailAsync(newItem);
                    itemsDictionary.AddOrUpdate(item.Id,
                        addValueFactory: k => item,
                        updateValueFactory: (k, existingItem) => existingItem);
                }
            });
        }

        private async Task<List<int>> GetHttpCall(List<int> newsIdsListCache, string endpoint)
        {
            var response = await _httpClient.GetAsync($"{endpoint}.json");
            var content = await response.Content.ReadAsStringAsync();
            newsIdsListCache = JsonConvert.DeserializeObject<List<int>>(content);
            return newsIdsListCache;
        }

        private async Task AddToDictionary(List<int> newsIdsListCache, ConcurrentDictionary<int, ItemEntity> itemsDictionary)
        {
            var optionsParallel = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 2 };
            await Parallel.ForEachAsync(newsIdsListCache, optionsParallel, async (news, token) =>
            {
                ItemEntity item = await GetItemDetailAsync(news);
                itemsDictionary.AddOrUpdate(item.Id,
                    addValueFactory: k => item,
                    updateValueFactory: (k, existingItem) => item);
            });
        }

        private static SearchResponse<List<ItemEntity>> PaginationPhase(int page, int pageSize, List<ItemEntity> listToReturn)
        {
            var totalCount = listToReturn.Count;
            var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
            var itemsPerPages = listToReturn
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new SearchResponse<List<ItemEntity>> {totalPages = totalPages, Data = itemsPerPages };
        }

        private async Task<Dictionary<int,ItemEntity>> LoadCache(ConcurrentDictionary<int, ItemEntity> concurrentDictionary, Dictionary<int, ItemEntity> returnDictionary, List<int> newsIdsListCache)
        {
            newsIdsListCache = await GetHttpCall(newsIdsListCache, "newstories");

            _memoryCache.Set("newscacheid", newsIdsListCache);

            await AddToDictionary(newsIdsListCache, concurrentDictionary);

            returnDictionary = concurrentDictionary.ToDictionary(pair => pair.Key, pair => pair.Value);
            _memoryCache.Set("newstories", returnDictionary);
            return returnDictionary;
        } 

        private async Task UpdateCache(ConcurrentDictionary<int, ItemEntity> itemsDictionary, Dictionary<int, ItemEntity> returnDictionary, List<int> newsIdsListCache)
        {
            List<int> newsIdsListIncome = new List<int>();
            newsIdsListIncome = await GetHttpCall(newsIdsListIncome, "newstories");

            newsIdsListCache = _memoryCache.Get<List<int>>("newscacheid");
            if (!(newsIdsListCache.Count == newsIdsListIncome.Count && newsIdsListCache.All(newsIdsListIncome.Contains)))
            {
                itemsDictionary = new ConcurrentDictionary<int, ItemEntity>(returnDictionary);
                await CheckAndAddToDictionary(itemsDictionary, returnDictionary, newsIdsListIncome);

                var listToRemove = newsIdsListCache.Except(newsIdsListIncome);
                returnDictionary = CheckAndRemoveFromDictionary(itemsDictionary, newsIdsListIncome, listToRemove);

                _memoryCache.Set("newstories", returnDictionary);
            }
        }
    }
}
