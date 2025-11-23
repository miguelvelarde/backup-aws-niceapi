using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace NiceAppApi.HealthCheck
{
    public class Service : IService
    {
        private readonly string _connectionString;

        public Service(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? configuration["DB_CONNECTION_STRING"] 
                ?? throw new InvalidOperationException("Database connection string not found");
        }

        public async Task<HealthStatus> CheckHealthAsync()
        {
            var stopwatch = Stopwatch.StartNew();
            var status = new HealthStatus
            {
                ApiGatewayStatus = "Healthy", // Si la Lambda se ejecutó, API Gateway está funcionando
                IsHealthy = false  // Inicialmente lo marcamos como no saludable
            };

            try
            {
                // Verificar conexión a la base de datos
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    
                    using var command = new MySqlCommand("SELECT 1", connection);
                    var result = await command.ExecuteScalarAsync();
                    
                    status.DatabaseStatus = "Healthy";
                    status.IsHealthy = true;
                }
            }
            catch (Exception ex)
            {
                status.DatabaseStatus = "Unhealthy";
                status.Message = $"Error de conexión: {ex.Message}";
            }

            stopwatch.Stop();
            status.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            
            return status;
        }
    }
}