using Domain.Services.Item;
using Models;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using ItemEntity = Domain.Data.Item;

namespace Domain.ItemServiceTest
{
    public class ItemServiceTests
    {
        private ItemService _itemService;
        private HttpClient _httpClient;
        private Mock<IMemoryCache> _memoryCache;

        [SetUp]
        public void Setup()
        {
            _httpClient = new HttpClient();
            _memoryCache = new Mock<IMemoryCache>();
            _itemService = new ItemService(_httpClient,_memoryCache.Object);
        }

        [Test]
        public async Task GetNewestStoriesAsync_WithEmptyCache_ReturnsFullList()
        {
            SearchRequest searchrequest = new SearchRequest();

            Mock<ICacheEntry> cacheEntry = new Mock<ICacheEntry>();

            Dictionary<int, ItemEntity> setDictionary = new Dictionary<int, ItemEntity>();
            List<int> idList = new List<int>();

            _memoryCache.Setup(cache => cache.TryGetValue("newstories", out It.Ref<object>.IsAny))
                   .Returns(true);
            _memoryCache.Setup(cache => cache.TryGetValue("newscacheid", out It.Ref<object>.IsAny))
                   .Returns(true);
            _memoryCache.Setup(cache => cache.CreateEntry("newscacheid")).Returns(cacheEntry.Object);
            _memoryCache.Setup(cache => cache.CreateEntry("newstories")).Returns(cacheEntry.Object);


            var result = await _itemService.GetNewestStoriesAsync(searchrequest);

            Assert.NotNull(result);
        }

        [Test]
        public async Task GetItemId_With_ReturnsItem()
        {
            Random random = new Random();
            int randomNumber = random.Next(1, 38000);

            var result = await _itemService.GetItemDetailAsync( randomNumber );

            Assert.NotNull(result);
        }
    }
}