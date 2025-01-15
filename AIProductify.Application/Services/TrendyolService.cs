using System.Net.Http.Headers;
using System.Text.Json;
using AIProductify.Application.Interfaces;
using AIProductify.Core.Entities;

namespace AIProductify.Application.Services
{
    public class TrendyolService : ITrendyolService
    {
        private readonly HttpClient _httpClient;
        private const string RootUrl = "https://api.trendyol.com/gateway-service/products";

        public TrendyolService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Product> GetProductDataAsync(string productId)
        {
            var url = $"{RootUrl}/{productId}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YourAccessToken");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Trendyol API error: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<Product>(content);

            return product;
        }
    }
}
