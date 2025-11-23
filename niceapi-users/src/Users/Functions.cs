using Amazon.Lambda.Core;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using NiceAppApi.Utils.Auth;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NiceApiUsers.Users
{
    public class Functions
    {
        private readonly IService _userService;

        public Functions(IService userService, JwtSettings jwtSettings)
        {
            _userService = userService;
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, "/users")]
        public async Task<APIGatewayHttpApiV2ProxyResponse> GetUser(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            string userId = "";
            if (request.QueryStringParameters != null && request.QueryStringParameters.ContainsKey("userId"))
                userId = request.QueryStringParameters["userId"];

            context.Logger.LogInformation($"Getting user with id: {userId}");

            try
            {
                var result = await _userService.GetUser(userId);
                if (!result.IsSuccess)
                {
                    return ResultHandler.CreateErrorResponse(404, result.Error);
                }
                return ResultHandler.CreateSuccessResponse(result.Value);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResultHandler.CreateErrorResponse(401, ex.Message);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error: {ex.Message}");
                return ResultHandler.CreateErrorResponse(500, "Internal server error");
            }
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Post, "/users")]
        public async Task<APIGatewayHttpApiV2ProxyResponse> CreateUser([FromBody] UserModel user, APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogInformation($"Creating new user: {user.Name}");

            try
            {
                var result = await _userService.CreateUserAsync(user);
                if (!result.IsSuccess)
                {
                    return ResultHandler.CreateErrorResponse(400, result.Error);
                }
                return ResultHandler.CreateSuccessResponse(result.Value);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResultHandler.CreateErrorResponse(401, ex.Message);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error: {ex.Message}");
                return ResultHandler.CreateErrorResponse(500, "Internal server error");
            }
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Put, "/users")]
        public async Task<APIGatewayHttpApiV2ProxyResponse> UpdateUser([FromBody] UserModel user, APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            string userId = user.UserId;
            context.Logger.LogInformation($"Updating user with Id: {user.UserId}");

            try
            {
                var result = await _userService.UpdateUserAsync(user);
                if (!result.IsSuccess)
                {
                    return ResultHandler.CreateErrorResponse(400, result.Error);
                }
                return ResultHandler.CreateSuccessResponse(result.Value);
            }
            catch (UnauthorizedAccessException ex)
            {
                return ResultHandler.CreateErrorResponse(401, ex.Message);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error: {ex.Message}");
                return ResultHandler.CreateErrorResponse(500, "Internal server error");
            }
        }
    }
}