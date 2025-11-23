using NiceAppApi.Utils.Auth;

namespace NiceApiUsers.Users
{
    public interface IService
    {
        Task<Result<UserModel>> GetUser(string userId);
        Task<Result<UserModel>> CreateUserAsync(UserModel user);
        Task<Result<UserModel>> UpdateUserAsync(UserModel user);
    }
}
