using AIProductify.Core.Entities;

namespace AIProductify.Application.Interfaces
{
    public interface IHtmlCrawlService
    {
        Task<Product> GetProductDataAsync(string productUrl);

    }
}
