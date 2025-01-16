using AIProductify.Application.DTO.Product;
using AIProductify.Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AIProductify.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {

        private readonly IHtmlCrawlService _htmlCrawlService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductController(IHtmlCrawlService htmlCrawlService, IProductService productService, IMapper mapper)
        {
            _htmlCrawlService = htmlCrawlService;
            _productService = productService;
            _mapper = mapper;
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
                var productEntity = await _htmlCrawlService.GetProductDataAsync(productUrl);

                var productDto = _mapper.Map<ProductDto>(productEntity);

                await _productService.SaveProductAsync(productDto);

                return Ok(productDto); 
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error while fetching the data: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
