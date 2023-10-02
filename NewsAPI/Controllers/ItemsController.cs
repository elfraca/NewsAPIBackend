using Domain.Services.Item;
using Microsoft.AspNetCore.Mvc;
using Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NewsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;
            
        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpPost("paged-search")]
        public async Task<IActionResult> GetPaginatedItems([FromBody] SearchRequest searchRequest)
        {
            var result = await _itemService.GetNewestStoriesAsync(searchRequest);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _itemService.GetItemDetailAsync(id);
            return Ok(result);
        }
    }
}
