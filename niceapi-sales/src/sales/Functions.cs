using Amazon.Lambda.Core;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using NiceAppApi.Utils.Auth;
using Amazon.Lambda.APIGatewayEvents;
using System.Security.Claims;
using NiceAppApi.Utils;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NiceAppApi.Sales{

    public class Functions : AuthorizedFunctionBase
    {
        private readonly IService _saleService;

        public Functions(IService saleService, JwtSettings jwtSettings)
            : base(jwtSettings)
        {
            _saleService = saleService;
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, "/sales/byuser/{records_to_return}")]
        public async Task<APIGatewayHttpApiV2ProxyResponse> GetSalesByUser(int records_to_return, APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            int userId = GetUserIdFromRequest(request, context);
            context.Logger.LogInformation($"Getting sales for user ID: {userId}");

            var result = await _saleService.GetSalesByUserIdAsync(userId, records_to_return);

            if (result.IsSuccess)
            {
                return CreateSuccessResponse(result.Value);
            }
            else
            {
                context.Logger.LogError($"Error: {result.Error}");
                return CreateErrorResponse(500, result.Error);
            }
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, "/sales/byclient/{clientId}/{records_to_return}")]
        public async Task<APIGatewayHttpApiV2ProxyResponse> GetSalesByClient(int clientId, int records_to_return, APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            int userId = GetUserIdFromRequest(request, context);
            context.Logger.LogInformation($"Getting sales for client ID: {clientId} for user ID: {userId}");

            var result = await _saleService.GetSalesByClientIdAsync(clientId, userId, records_to_return);

            if (result.IsSuccess)
            {
                return CreateSuccessResponse(result.Value);
            }
            else
            {
                context.Logger.LogError($"Error: {result.Error}");
                return CreateErrorResponse(500, result.Error);
            }
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, "/sales/byid/{saleId}")]
        public async Task<APIGatewayHttpApiV2ProxyResponse> GetSale(int saleId, APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            int userId = GetUserIdFromRequest(request, context);
            context.Logger.LogInformation($"Getting sale with ID: {saleId} for user ID: {userId}");

            var result = await _saleService.GetSaleByIdAsync(saleId, userId);

            if (result.IsSuccess)
            {
                if (result.Value == null)
                {
                    return CreateErrorResponse(404, "Venta no encontrada");
                }

                return CreateSuccessResponse(result.Value);
            }
            else
            {
                context.Logger.LogError($"Error: {result.Error}");
                return CreateErrorResponse(500, result.Error);
            }
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Post, "/sales")]
        public async Task<APIGatewayHttpApiV2ProxyResponse> CreateSale([FromBody] SaleModel sale, APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogInformation($"Headers Request: {string.Join(", ", request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");

            int userId = GetUserIdFromRequest(request, context);
            context.Logger.LogInformation($"Creating new sale for product ID: {sale.ProductId} for user ID: {userId}");
            sale.UserId = userId;

            var result = await _saleService.CreateSaleAsync(sale);

            if (result.IsSuccess)
            {
                return CreateSuccessResponse(result.Value);
            }
            else
            {
                context.Logger.LogError($"Error: {result.Error}");
                return CreateErrorResponse(500, result.Error);
            }
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Put, "/sales")]
        public async Task<APIGatewayHttpApiV2ProxyResponse> UpdateSale([FromBody] SaleModel sale, APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            int userId = GetUserIdFromRequest(request, context);
            context.Logger.LogInformation($"Updating sale ID: {sale.SaleId} for user ID: {userId}");
            sale.UserId = userId;

            var result = await _saleService.UpdateSaleAsync(sale);

            if (result.IsSuccess)
            {
                return CreateSuccessResponse(result.Value);
            }
            else
            {
                context.Logger.LogError($"Error: {result.Error}");
                return CreateErrorResponse(500, result.Error);
            }
        }
    }
}