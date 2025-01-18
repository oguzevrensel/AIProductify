using AIProductify.Core.Entities;

namespace AIProductify.Application.Interfaces
{
    public interface IAiService
    {
        Task<decimal> CalculateProductScoreAsync(Product product);
    }
}
