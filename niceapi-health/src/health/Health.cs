using Amazon.Lambda.Core;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;


[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NiceAppApi.HealthCheck
{
    public class Health
    {
        private readonly IService _healthCheckService;

        public Health(IService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, "/health")]
        public async Task<HealthStatus> CheckHealth(ILambdaContext context)
        {
            context.Logger.LogInformation("Ejecutando health check");

            var result = await _healthCheckService.CheckHealthAsync();

            if (result.IsHealthy)
                context.Logger.LogInformation("Health check completado con éxito: Healthy");
            else
                context.Logger.LogWarning($"Health check falló: {result.Message}");

            return result;
        }

        
        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Post, "/echo")]
        public APIGatewayHttpApiV2ProxyResponse Echo(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogInformation("Ejecutando método de eco");
            
            // Log del body recibido
            context.Logger.LogInformation($"Body recibido: {request.Body}");
            
            // Crear respuesta con el mismo cuerpo
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = request.Body,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "application/json" },
                    { "X-Echo-Test", "true" }
                }
            };
        }
    }
}