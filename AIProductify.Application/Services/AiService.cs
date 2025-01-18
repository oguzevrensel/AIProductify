using AIProductify.Application.Interfaces;
using AIProductify.Core.Entities;

namespace AIProductify.Application.Services
{
    public class AiService : IAiService
    {
        private readonly IOpenAiService _openAiService;

        public AiService(IOpenAiService openAiService)
        {
            _openAiService = openAiService;
        }

        public async Task<decimal> CalculateProductScoreAsync(Product product)
        {
            var prompt = $@"
                Evaluate the following product based on:
                1. Clarity and descriptiveness of the name.
                2. Detail and SEO compliance of the description.
                3. Number of images.
                4. Richness of attributes and brand details.
                Provide a score between 0 and 100. Only give me score. Do not write anything except score.
                Score value should be decimal. I need your response only between 0 and 100

                Product Details:
                Name: {product.Name}
                Description: {product.Description}
                Sku: {product.Sku}
                ParentSku: {product.ParentSku}
                Original Price: {product.OriginalPrice}
                Discounted Price: {product.DiscountedPrice}
                Attributes: {string.Join(", ", product.Attributes.Select(a => $"{a.Key}: {a.Name}"))}
                Brand: {product.Brand}
                Category: {product.Category}
                Images: {product.Images?.Count ?? 0}";

            var response = await _openAiService.GenerateResponseAsync(prompt);

            if (decimal.TryParse(response, out var score))
            {
                return score;
            }

            throw new InvalidOperationException("AI score response could not be parsed as a decimal value.Score:" + score);

        }
    }

}
