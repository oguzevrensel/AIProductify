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
        private readonly ITranslationService _translationService;
        private readonly IAiService _aiService;


        private readonly IMapper _mapper;

        public ProductController(IHtmlCrawlService htmlCrawlService, IProductService productService, ITranslationService translationService, IAiService aiService, IMapper mapper)
        {
            _htmlCrawlService = htmlCrawlService;
            _productService = productService;
            _translationService = translationService;
            _aiService = aiService;
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


        [HttpGet("list-crawled-products")]
        public async Task<IActionResult> ListCrawledProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();

                if (products == null || !products.Any())
                {
                    return NotFound("No products found.");
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while fetching products: {ex.Message}");
            }
        }


        [HttpGet("transform-product")]
        public async Task<IActionResult> TransformProduct([FromQuery] string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
            {
                return BadRequest("SKU is required.");
            }

            try
            {
                var product = await _productService.GetProductBySkuAsync(sku);

                if (product == null)
                {
                    return NotFound($"Product with SKU {sku} not found.");
                }

                //Translate English to Product
                var translatedProduct = await _translationService.TranslateProductAsync(product);

                //Get Score for the Product
                var score = await _aiService.CalculateProductScoreAsync(translatedProduct);

                var response = new
                {
                    name = translatedProduct.Name,
                    description = translatedProduct.Description,
                    sku = translatedProduct.Sku,
                    parentSku = string.Join(",", translatedProduct.ParentSku ?? new List<string>()),
                    attributes = translatedProduct.Attributes != null
                        ? translatedProduct.Attributes.Select(a => new { key = a.Key, name = a.Name }).Cast<object>().ToList()
                        : new List<object>(),
                    category = translatedProduct.Category,
                    brand = translatedProduct.Brand,
                    originalPrice = translatedProduct.OriginalPrice,
                    discountedPrice = translatedProduct.DiscountedPrice,
                    images = translatedProduct.Images,
                    score = score
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


    }

}
