using Domain.Services.Item;
using Microsoft.AspNetCore.Mvc;
using Models;
using Moq;
using NewsAPI.Controllers;
using ItemEntity = Domain.Data.Item;

namespace NewsAPI.Test
{
    public class ItemsControllerTests
    {
        Mock<IItemService> _mockItemService;
        ItemsController _itemsController;

        [SetUp]
        public void Setup()
        {
            _mockItemService = new Mock<IItemService>();
            _itemsController = new ItemsController(_mockItemService.Object);
        }

        [Test]
        public async Task GetItemDetailById_withRandomNumber_returnsItem()
        {
            ItemEntity item = new ItemEntity();
            _mockItemService.Setup(service => service.GetItemDetailAsync(It.IsAny<int>())).ReturnsAsync(item);

            var result = await _itemsController.GetById(It.IsAny<int>());


            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetPaginatedItems_withRandomNumber_returnsItem()
        {
            SearchResponse<List<ItemEntity>> response = new SearchResponse<List<ItemEntity>>();
            ItemEntity item = new ItemEntity();
            _mockItemService.Setup(service => service.GetNewestStoriesAsync(It.IsAny<SearchRequest>())).ReturnsAsync(response);

            var result = await _itemsController.GetPaginatedItems(It.IsAny<SearchRequest>());


            Assert.IsInstanceOf<OkObjectResult>(result);
        }
    }
}