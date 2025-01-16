
using AIProductify.Application.DTO.Product;

namespace AIProductify.Application.Interfaces
{
    public interface IProductService
    {
        Task SaveProductAsync(ProductDto product);

    }
}
