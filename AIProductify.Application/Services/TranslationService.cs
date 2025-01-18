using AIProductify.Application.Interfaces;
using AIProductify.Core.Entities;

namespace AIProductify.Application.Services
{
    public class TranslationService : ITranslationService
    {
        private readonly IOpenAiService _openAiService;

        public TranslationService(IOpenAiService openAiService)
        {
            _openAiService = openAiService;
        }

        public async Task<Product> TranslateProductAsync(Product product)
        {
            // Ürün özelliklerini çevir
            product.Name = await _openAiService.TranslateTextAsync(product.Name ?? string.Empty);
            product.Description = await _openAiService.TranslateTextAsync(product.Description ?? string.Empty);

            foreach (var attribute in product.Attributes ?? new List<ProductAttribute>())
            {
                attribute.Key = await _openAiService.TranslateTextAsync(attribute.Key);
                attribute.Name = await _openAiService.TranslateTextAsync(attribute.Name);
            }

            product.Category = await _openAiService.TranslateTextAsync(product.Category ?? string.Empty);
            product.Brand = await _openAiService.TranslateTextAsync(product.Brand ?? string.Empty);

            return product;
        }
    }

}
