using NiceApiStock.Models;
using NiceApp.Common.Models;

namespace NiceApiStock.Services
{
    public interface IService
    {
        Task<Result<StockModel>> GetAsync(string stockId, string userId);
        Task<Result<StockModel>> CreateAsync(StockModel stock);
        Task<Result<StockModel>> UpdateAsync(StockModel stock);
    }
}