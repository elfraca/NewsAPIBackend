using Domain.Services.Item;
using Models;
using Microsoft.Extensions.Caching.Memory;
using Moq;

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
        public async Task GetNewestStoriesAsync()
        {
            SearchRequest searchrequest = new SearchRequest();
            var result = await _itemService.GetNewestStoriesAsync(searchrequest);

            Assert.NotNull(result);
        }
    }
}