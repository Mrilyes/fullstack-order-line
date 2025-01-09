using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using order_orderline.Application.DTOs;
using order_orderline.Application.Services;

namespace order_orderline.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _service;

        public ArticleController(IArticleService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var articles = await _service.GetAllArticlesAsync();
            return Ok(articles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var article = await _service.GetArticleByIdAsync(id);
            if (article == null)
                return NotFound();

            return Ok(article);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ArticleDto articleDto)
        {
            await _service.CreateArticleAsync(articleDto);
            return CreatedAtAction(nameof(GetById), new { id = articleDto.ArticleId }, articleDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ArticleDto articleDto)
        {
            if (id != articleDto.ArticleId)
                return BadRequest();

            await _service.UpdateArticleAsync(articleDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteArticleAsync(id);
            return NoContent();
        }
    }
}
