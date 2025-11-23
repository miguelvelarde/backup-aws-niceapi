using NiceAppApi.Utils.Auth;

namespace NiceApiUsers.Users
{
    public interface IUserRepository
    {
        Task<UserModel?> GetByIdAsync(string userId);
        Task<UserModel?> CreateAsync(UserModel user);
        Task<UserModel?> UpdateAsync(UserModel user);
    }
}