using AIProductify.Core.Entities;

namespace AIProductify.Application.Interfaces
{
    public interface ITranslationService
    {
        Task<Product> TranslateProductAsync(Product product);
    }
}
