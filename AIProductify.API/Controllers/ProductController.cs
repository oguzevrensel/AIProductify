using AIProductify.API.Helper;
using AIProductify.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AIProductify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {

        private readonly IHtmlCrawlService _htmlCrawlService;

        public ProductController(IHtmlCrawlService htmlCrawlService)
        {
            _htmlCrawlService = htmlCrawlService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet("crawl-product")]
        public async Task<IActionResult> CrawlProduct([FromQuery] string productUrl)
        {
            if (string.IsNullOrWhiteSpace(productUrl))
            {
                return BadRequest("Product URL is required.");
            }

            try
            {
                
                var product = await _htmlCrawlService.GetProductDataAsync(productUrl);

                return Ok(product);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error while fetching the data: {ex.Message}");
            }
        }


        


        
    }
}
