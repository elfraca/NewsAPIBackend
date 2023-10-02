using Domain.Services.Item;
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
        public void Test1()
        {
            Assert.Pass();
        }
    }
}