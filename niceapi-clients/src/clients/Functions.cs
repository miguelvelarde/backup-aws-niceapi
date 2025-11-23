using Amazon.Lambda.Core;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using NiceAppApi.Utils.Auth;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NiceAppApi.Clients
{
    public class Functions : AuthorizedFunctionBase
    {
        private readonly IService _clientService;

        public Functions(IService clientService, JwtSettings jwtSettings)
            : base(jwtSettings)
        {
            _clientService = clientService;
        }


        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, "/clients")]
        
        public async Task<APIGatewayHttpApiV2ProxyResponse> GetClient([FromBody] ClientModel model, APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {            
            model.UserId = GetUserIdFromRequest(request, context);

            context.Logger.LogInformation($"Getting clients for user {model.UserId}");

            var result = await _clientService.GetClientsAsync(model.UserId, model.ClientId, model.Name, model.Phone);

            if (result.IsSuccess)
            {
                return CreateSuccessResponse(result.Value);
            }

            return CreateErrorResponse(500, result.Error);
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Post, "/clients")]
        public async Task<APIGatewayHttpApiV2ProxyResponse> CreateClient([FromBody] ClientModel client, APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            int currentUser = GetUserIdFromRequest(request, context);
            client.UserId = currentUser;

            context.Logger.LogInformation($"Creating new client: {client.Name} for user {currentUser}");

            var result = await _clientService.CreateClientAsync(client);

            if (result.IsSuccess)
            {
                return CreateSuccessResponse(result.Value);
            }

            return CreateErrorResponse(500, result.Error);
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Put, "/clients")]
        public async Task<APIGatewayHttpApiV2ProxyResponse> UpdateClient([FromBody] ClientModel client, APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            int currentUser = GetUserIdFromRequest(request, context);
            client.UserId = currentUser;

            context.Logger.LogInformation($"Updating client ID: {client.ClientId} for user {currentUser}");

            var result = await _clientService.UpdateClientAsync(client);

            if (result.IsSuccess)
            {
                return CreateSuccessResponse(result.Value);
            }

            return CreateErrorResponse(500, result.Error);
        }
    }
}