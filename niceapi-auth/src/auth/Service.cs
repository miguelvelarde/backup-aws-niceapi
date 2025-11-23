using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using NiceAppApi.Utils.Auth;

namespace NiceAppApi.Auth;

public class Service : IService
{
    private readonly string _connectionString;
    private readonly string _secretKey;
    private readonly int _tokenExpiryMinutes;

    public Service(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration["DATABASE_CONNECTION_STRING"]
            ?? throw new InvalidOperationException("Database connection string not found");

        // JWT Secret Key - en producción debe estar en secretos o configuración segura
        _secretKey = configuration["JWT_SECRET_KEY"] ?? "NiceApp-SuperSecretKey-12345!@#$%";
        _tokenExpiryMinutes = int.TryParse(configuration["JWT_EXPIRY_MINUTES"], out int mins) ? mins : 120; // 2 horas por defecto
    }

    private async Task<UserModel> GetUser(int userId)
    {
        using var connection = new MySqlConnection(_connectionString);
        using var command = new MySqlCommand("CALL sp_UsersGetById(@UserId)", connection);

        command.Parameters.AddWithValue("@UserId", userId);

        await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapToUser((MySqlDataReader)reader);
        }

        return null;
    }

    private static UserModel MapToUser(MySqlDataReader reader)
        {
            return new UserModel
            {
                UserId = (int)reader["UserId"],
                Phone = reader["Phone"].ToString(),
                Name = reader["Name"].ToString(),
                Password = reader["Password"].ToString(),
                Type = reader["Type"].ToString(),
                Team = reader["Team"].ToString(),
                Selfie = reader["Selfie"].ToString(),
                Status = reader["Status"] != DBNull.Value ? Convert.ToInt16(reader["Status"]) : (short)0
            };
        }

    public async Task<Result<LoginResult>> LoginAsync(int userId, string password)
    {
        var user = await GetUser(userId);

        if (user == null)
            return Result<LoginResult>.Failure("Usuario no encontrado");

        if (user.Status != 1)
            return Result<LoginResult>.Failure("Usuario inactivo o suspendido");

        if (!Tokenizer.VerifyPassword(password, user.Password))
            return Result<LoginResult>.Failure("Contrasena incorrecta");

        // Si la autenticación es exitosa, generamos el token
        var token = Tokenizer.GenerateJwtToken(user, _secretKey, _tokenExpiryMinutes);

        // No devolvemos la contraseña en la respuesta
        user.Password = string.Empty;

        return Result<LoginResult>.Success(new LoginResult
        {
            Success = true,
            Token = token,
            Expiry = DateTime.UtcNow.AddMinutes(_tokenExpiryMinutes)
        });
    }
    
}