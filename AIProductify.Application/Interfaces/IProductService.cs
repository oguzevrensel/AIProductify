
using AIProductify.Application.DTO.Product;
using AIProductify.Core.Entities;

namespace AIProductify.Application.Interfaces
{
    public interface IProductService
    {
        Task SaveProductAsync(ProductDto product);
        Task<List<ProductDto>> GetAllProductsAsync();

        Task<Product> GetProductBySkuAsync(string sku);

    }
}
