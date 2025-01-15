using AIProductify.Core.Entities;

namespace AIProductify.Application.Interfaces
{
    public interface ITrendyolService
    {
        Task<Product> GetProductDataAsync(string productId);
    }
}
