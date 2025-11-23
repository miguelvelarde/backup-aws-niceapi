
using NiceAppApi.Utils.Auth;

namespace NiceApiUsers.Users;

public class Service : IService
{
    private readonly IUserRepository _userRepository;

    public Service(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserModel>> GetUser(string userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Result<UserModel>.Failure("User not found");
            }
            user.Password = string.Empty;
            return Result<UserModel>.Success(user);
        }
        catch (Exception ex)
        {
            return Result<UserModel>.Failure(ex.Message);
        }
    }

    public async Task<Result<UserModel>> CreateUserAsync(UserModel user)
    {
        try
        {
            ValidateModel(user);

            var userExists = await _userRepository.GetByIdAsync(user.UserId);

            if (userExists != null)
            {
                return Result<UserModel>.Failure("Este teléfono o usuario ya está registrado.");
            }

            user.Password = Tokenizer.HashPassword(user.Password);

            await _userRepository.CreateAsync(user);

            user.Password = string.Empty;
            return Result<UserModel>.Success(user);
        }
        catch (Exception ex)
        {
            return Result<UserModel>.Failure(ex.Message);
        }
    }

    public async Task<Result<UserModel>> UpdateUserAsync(UserModel user)
    {
        try
        {
            ValidateModel(user);
            user.Password = Tokenizer.HashPassword(user.Password);
            await _userRepository.UpdateAsync(user);
            user.Password = string.Empty;

            return Result<UserModel>.Success(user);
        }
        catch (Exception ex)
        {
            return Result<UserModel>.Failure(ex.Message);
        }
    }

    private static void ValidateModel(UserModel user)
    {
        if (string.IsNullOrEmpty(user.UserId))
            throw new ArgumentException("El teléfono no puede estar vacío");

        if (string.IsNullOrEmpty(user.Password))
            throw new ArgumentException("El password no puede estar vacío");

        if (string.IsNullOrEmpty(user.Name))
            throw new ArgumentException("El nombre no puede estar vacío");

        if (string.IsNullOrEmpty(user.Type))
            throw new ArgumentException("El tipo de usuario no puede estar vacío");

        if (user.Type != "user" && user.Type != "admin")
            throw new ArgumentException("El tipo de usuario debe ser 'user' o 'admin'");

        if (string.IsNullOrEmpty(user.Team))
            throw new ArgumentException("El equipo no puede estar vacío");

        if (string.IsNullOrEmpty(user.Selfie))
            throw new ArgumentException("La URL de la selfie no puede estar vacía");
    }
}
