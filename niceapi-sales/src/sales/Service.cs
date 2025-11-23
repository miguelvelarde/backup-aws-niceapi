using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using NiceAppApi.Utils;

namespace NiceAppApi.Sales
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

        public async Task<Result<IEnumerable<SaleDetailModel>>> GetSalesByUserIdAsync(int userId, int records)
        {
            try
            {
                var sales = new List<SaleDetailModel>();

                using (var connection = new MySqlConnection(_connectionString))
                {
                    using var command = new MySqlCommand(
                        "CALL sp_SalesGetByUser(@UserId, @Records)",
                        connection);

                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Records", records);

                    await connection.OpenAsync();

                    using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        sales.Add(MapToSaleDetail((MySqlDataReader)reader));
                    }
                }

                return Result<IEnumerable<SaleDetailModel>>.Success(sales);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<SaleDetailModel>>.Failure($"Error al obtener ventas: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<SaleDetailModel>>> GetSalesByClientIdAsync(int clientId, int userId, int records)
        {
            try
            {
                var sales = new List<SaleDetailModel>();

                using (var connection = new MySqlConnection(_connectionString))
                {
                    using var command = new MySqlCommand(
                        "CALL sp_SalesGetByClient(@ClientId, @UserId, @Records)",
                        connection);

                    command.Parameters.AddWithValue("@ClientId", clientId);
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Records", records);

                    await connection.OpenAsync();

                    using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        sales.Add(MapToSaleDetail((MySqlDataReader)reader));
                    }
                }

                return Result<IEnumerable<SaleDetailModel>>.Success(sales);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<SaleDetailModel>>.Failure($"Error al obtener ventas por cliente: {ex.Message}");
            }
        }

        public async Task<Result<SaleDetailModel>> GetSaleByIdAsync(int saleId, int userId)
        {
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                using var command = new MySqlCommand("CALL sp_SalesGetBySaleId(@UserId, @SaleId)", connection);

                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@SaleId", saleId);

                await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return Result<SaleDetailModel>.Success(MapToSaleDetail((MySqlDataReader)reader));
                }

                return Result<SaleDetailModel>.Success(null);
            }
            catch (Exception ex)
            {
                return Result<SaleDetailModel>.Failure($"Error al obtener la venta: {ex.Message}");
            }
        }

        public async Task<Result<SaleModel>> CreateSaleAsync(SaleModel sale)
        {
            try
            {
                ValidateSale(sale);

                using var connection = new MySqlConnection(_connectionString);
                using var command = new MySqlCommand(
                    "CALL sp_SalesCreate(@ClientId, @UserId, @ProductId, @Quantity, @Price, @Comments, @NoTicket, @SaleType)",
                    connection);

                command.Parameters.AddWithValue("@ClientId", sale.ClientId.HasValue ? (object)sale.ClientId : DBNull.Value);
                command.Parameters.AddWithValue("@UserId", sale.UserId);
                command.Parameters.AddWithValue("@ProductId", sale.ProductId);
                command.Parameters.AddWithValue("@Quantity", sale.Quantity);
                command.Parameters.AddWithValue("@Price", sale.Price);
                command.Parameters.AddWithValue("@Comments", sale.Comments ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@NoTicket", sale.NoTicket);
                command.Parameters.AddWithValue("@SaleType", string.IsNullOrEmpty(sale.SaleType) ? "Contado" : sale.SaleType);

                await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    sale = MapToSale((MySqlDataReader)reader);
                }

                return Result<SaleModel>.Success(sale);
            }
            catch (Exception ex)
            {
                return Result<SaleModel>.Failure($"Error al crear la venta: {ex.Message}");
            }
        }

        public async Task<Result<SaleModel>> UpdateSaleAsync(SaleModel sale)
        {
            try
            {
                ValidateSale(sale);

                using var connection = new MySqlConnection(_connectionString);
                using var command = new MySqlCommand(
                    "CALL sp_SalesUpdate(@SaleId, @Price, @Status, @Comments)",
                    connection);

                command.Parameters.AddWithValue("@SaleId", sale.SaleId);
                command.Parameters.AddWithValue("@Price", sale.Price);
                command.Parameters.AddWithValue("@Status", string.IsNullOrEmpty(sale.Status) ? "completed" : sale.Status);
                command.Parameters.AddWithValue("@Comments", sale.Comments ?? (object)DBNull.Value);

                await connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    sale = MapToSale((MySqlDataReader)reader);
                }

                return Result<SaleModel>.Success(sale);
            }
            catch (MySqlException ex) when (ex.Message.Contains("La venta no existe"))
            {
                return Result<SaleModel>.Failure("La venta no existe");
            }
            catch (Exception ex)
            {
                return Result<SaleModel>.Failure($"Error al actualizar la venta: {ex.Message}");
            }
        }

        private static SaleModel MapToSale(MySqlDataReader reader)
        {
            return new SaleModel
            {
                SaleId = Convert.ToInt32(reader["SaleId"]),
                SaleDate = Convert.ToDateTime(reader["SaleDate"]),
                ClientId = reader["ClientId"] != DBNull.Value ? Convert.ToInt32(reader["ClientId"]) : null,
                UserId = Convert.ToInt32(reader["UserId"]),
                ProductId = Convert.ToInt32(reader["ProductId"]),
                Quantity = Convert.ToInt32(reader["Quantity"]),
                Price = Convert.ToDecimal(reader["Price"]),
                Total = Convert.ToDecimal(reader["Total"]),
                Status = reader["Status"].ToString(),
                Comments = reader["Comments"] != DBNull.Value ? reader["Comments"].ToString() : null,
                NoTicket = Convert.ToInt32(reader["NoTicket"]),
                SaleType = reader["SaleType"].ToString()
            };
        }

        private static SaleDetailModel MapToSaleDetail(MySqlDataReader reader)
        {
            return new SaleDetailModel
            {
                SaleId = Convert.ToInt32(reader["SaleId"]),
                SaleDate = Convert.ToDateTime(reader["SaleDate"]),
                ClientId = reader["ClientId"] != DBNull.Value ? Convert.ToInt32(reader["ClientId"]) : null,
                UserId = Convert.ToInt32(reader["UserId"]),
                ProductId = Convert.ToInt32(reader["ProductId"]),
                ProductName = reader["Name"].ToString(),
                ProductDescription = reader["Description"].ToString(),
                ProductImage = reader["Image"].ToString(),
                ProductType = reader["Type"].ToString(),
                Quantity = Convert.ToInt32(reader["Quantity"]),
                Price = Convert.ToDecimal(reader["Price"]),
                Total = Convert.ToDecimal(reader["Total"]),
                Status = reader["Status"].ToString(),
                Comments = reader["Comments"] != DBNull.Value ? reader["Comments"].ToString() : null,
                NoTicket = Convert.ToInt32(reader["NoTicket"]),
                SaleType = reader["SaleType"].ToString()
            };
        }

        private static void ValidateSale(SaleModel sale)
        {
            if (sale == null)
                throw new ArgumentNullException(nameof(sale), "La venta no puede ser nula");

            if (sale.UserId <= 0)
                throw new ArgumentException("El ID del usuario debe ser mayor que cero");

            if (sale.ProductId <= 0)
                throw new ArgumentException("El ID del producto debe ser mayor que cero");

            if (sale.Quantity <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que cero");

            if (sale.Price <= 0)
                throw new ArgumentException("El precio debe ser mayor que cero");

            if (sale.NoTicket <= 0)
                throw new ArgumentException("El número de ticket debe ser mayor que cero");

            // Para SaleType, verificar que sea uno de los valores del ENUM ('Contado', 'Credito')
            if (!string.IsNullOrEmpty(sale.SaleType) &&
                sale.SaleType != "Contado" && sale.SaleType != "Credito")
            {
                throw new ArgumentException("Tipo de venta inválido. Valores permitidos: Contado, Credito");
            }

            // Para Status, verificar que sea uno de los valores del ENUM ('completed', 'canceled', 'returned')
            if (!string.IsNullOrEmpty(sale.Status) &&
                sale.Status != "completed" && sale.Status != "canceled" && sale.Status != "returned")
            {
                throw new ArgumentException("Estado inválido. Valores permitidos: completed, canceled, returned");
            }
        }
    }

}