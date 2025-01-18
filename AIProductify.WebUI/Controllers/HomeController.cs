using System.Diagnostics;
using AIProductify.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIProductify.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("LongRunningClient");
            _httpClient.BaseAddress = new Uri("https://localhost:44388/api/"); // API'nin base URL'si

            var apiKey = configuration["ApiSettings:ApiKey"];
            if (!string.IsNullOrEmpty(apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrawlProduct(string productUrl)
        {
            if (string.IsNullOrWhiteSpace(productUrl))
            {
                ViewBag.Message = "Product URL is required.";
                return View("Index");
            }

            try
            {
                var response = await _httpClient.GetAsync($"Product/crawl-product?productUrl={productUrl}");
                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Product successfully crawled!";
                }
                else
                {
                    ViewBag.Message = $"Failed to crawl product: {await response.Content.ReadAsStringAsync()}";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
            }

            return View("Index");
        }


        public async Task<IActionResult> ListCrawledProducts()
        {
            try
            {
                var response = await _httpClient.GetAsync("Product/list-crawled-products");
                if (response.IsSuccessStatusCode)
                {
                    var products = await response.Content.ReadFromJsonAsync<List<ProductViewModel>>();
                    return View(products);
                }
                else
                {
                    ViewBag.Message = "Failed to load products.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
            }

            return View(new List<ProductViewModel>());
        }


        public async Task<IActionResult> ShowProduct(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
            {
                return RedirectToAction("ListCrawledProducts");
            }

            try
            {
                var response = await _httpClient.GetAsync($"Product/transform-product?sku={sku}");
                if (response.IsSuccessStatusCode)
                {
                    var product = await response.Content.ReadFromJsonAsync<ProductViewModel>();
                    return View(product);
                }
                else
                {
                    ViewBag.Message = "Failed to load product details.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("ListCrawledProducts");
        }
    }
}
