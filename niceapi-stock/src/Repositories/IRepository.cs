using NiceApiStock.Models;

namespace NiceApiStock.Repositories
{
    public interface IRepository
    {
        Task<StockModel> GetByIdAsync(string stockId, string userId);
        Task<StockModel> CreateAsync(StockModel stock);
        Task<StockModel> UpdateAsync(StockModel stock);
    }
}
