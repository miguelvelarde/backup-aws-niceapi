namespace NiceAppApi.Auth;

public interface IService
{
    Task<Result<LoginResult>> LoginAsync(int userId, string password);
}