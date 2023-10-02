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

        // GET: api/<ValuesController>
        [HttpPost("paged-search")]
        public async Task<IActionResult> GetPaginatedItems([FromBody] SearchRequest searchRequest)
        {
            var result = await _itemService.GetNewestStoriesAsync(searchRequest);
            return Ok(result);
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _itemService.GetItemDetailAsync(id);
            return Ok(result);
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
