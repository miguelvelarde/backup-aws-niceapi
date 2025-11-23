using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace NiceAppApi.Clients
{
    public class Service : IService
    {
        private readonly string _connectionString;

        public Service(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? configuration["DATABASE_CONNECTION_STRING"]
                ?? throw new InvalidOperationException("Database connection string not found");
        }

        public async Task<Result<IEnumerable<ClientModel>>> GetClientsAsync(int userId, int? clientId, string name, string phone)
        {
            try
            {
                var result = new List<ClientModel>();

                using (var connection = new MySqlConnection(_connectionString))
                {
                    using var command = new MySqlCommand("CALL sp_ClientsGet(@ClientId, @UserId, @Name, @Phone)", connection);
                    command.Parameters.AddWithValue("@ClientId", clientId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Name", name ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Phone", phone ?? (object)DBNull.Value);

                    await connection.OpenAsync();

                    using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        result.Add(MapToClient((MySqlDataReader)reader));
                    }
                }

                return Result<IEnumerable<ClientModel>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<ClientModel>>.Failure(ex.Message);
            }
        }

        public async Task<Result<ClientModel>> CreateClientAsync(ClientModel client)
        {
            try
            {
                ValidateClient(client);

                using var connection = new MySqlConnection(_connectionString);
                using var command = new MySqlCommand("CALL sp_ClientsCreate(@Name, @Phone, @Comments, @UserId)", connection);

                command.Parameters.AddWithValue("@Name", client.Name);
                command.Parameters.AddWithValue("@Phone", client.Phone);
                command.Parameters.AddWithValue("@Comments", client.Comments ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UserId", client.UserId);

                await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return Result<ClientModel>.Success(MapToClient((MySqlDataReader)reader));
                }

                return Result<ClientModel>.Failure("No se pudo crear el cliente");
            }
            catch (Exception ex)
            {
                return Result<ClientModel>.Failure(ex.Message);
            }
        }

        public async Task<Result<ClientModel>> UpdateClientAsync(ClientModel client)
        {
            try
            {
                ValidateClient(client);

                using var connection = new MySqlConnection(_connectionString);
                using var command = new MySqlCommand("CALL sp_ClientsUpdate(@Name, @Phone, @Comments, @UserId, @ClientId)", connection);

                command.Parameters.AddWithValue("@Name", client.Name);
                command.Parameters.AddWithValue("@Phone", client.Phone);
                command.Parameters.AddWithValue("@Comments", client.Comments ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UserId", client.UserId);
                command.Parameters.AddWithValue("@ClientId", client.ClientId);

                await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return Result<ClientModel>.Success(MapToClient((MySqlDataReader)reader));
                }

                return Result<ClientModel>.Failure("No se pudo actualizar el cliente");
            }
            catch (Exception ex)
            {
                return Result<ClientModel>.Failure(ex.Message);
            }
        }

        private static ClientModel MapToClient(MySqlDataReader reader)
        {
            return new ClientModel
            {
                ClientId = reader.GetInt32("ClientId"),
                Name = reader["Name"].ToString(),
                Phone = reader["Phone"].ToString(),
                Comments = reader["Comments"] != DBNull.Value ? reader["Comments"].ToString() : null,
                // El UserId podría no incluirse en el resultado de algunos procedimientos almacenados
                // Verificamos si la columna existe antes de intentar asignarla
                UserId = reader.GetOrdinal("UserId") >= 0 ? reader.GetInt32("UserId") : 0
            };
        }

        private static void ValidateClient(ClientModel client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client), "El cliente no puede ser nulo");

            if (string.IsNullOrEmpty(client.Name))
                throw new ArgumentException("El nombre del cliente no puede estar vacío");

            if (string.IsNullOrEmpty(client.Phone))
                throw new ArgumentException("El teléfono del cliente no puede estar vacío");

            if (client.UserId <= 0)
                throw new ArgumentException("El ID del usuario debe ser mayor que cero");

        }
    }
}