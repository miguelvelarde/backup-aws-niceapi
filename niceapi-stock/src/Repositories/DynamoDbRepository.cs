using Amazon.DynamoDBv2.DataModel;
using NiceApiStock.Models;

namespace NiceApiStock.Repositories
{
    public class DynamoDbRepository : IRepository
    {
        private readonly IDynamoDBContext _dbContext;

        public DynamoDbRepository(IDynamoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<StockModel> GetByIdAsync(string stockId, string userId)
            => await _dbContext.LoadAsync<StockModel>(userId, stockId);

        public async Task<StockModel> CreateAsync(StockModel stock)
        {
            await _dbContext.SaveAsync(stock);
            return stock;
        }

        public async Task<StockModel> UpdateAsync(StockModel stock)
        {
            await _dbContext.SaveAsync(stock);
            return stock;
        }
    }
}