using Amazon.DynamoDBv2.DataModel;
using NiceAppApi.Utils.Auth;

namespace NiceApiUsers.Users
{
    public class DynamoDbUserRepository : IUserRepository
    {
        private readonly IDynamoDBContext _dbContext;

        public DynamoDbUserRepository(IDynamoDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserModel?> GetByIdAsync(string userId)
            => await _dbContext.LoadAsync<UserModel>(userId);

        public async Task<UserModel?> CreateAsync(UserModel user)
        {
            await _dbContext.SaveAsync(user);
            return user;
        }

        public async Task<UserModel?> UpdateAsync(UserModel user)
        {
            await _dbContext.SaveAsync(user);
            return user;
        }
    }
}