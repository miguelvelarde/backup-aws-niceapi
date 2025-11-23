using Amazon.Lambda.Core;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.APIGatewayEvents;
using NiceAppApi.Utils.Auth;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NiceAppApi.Auth
{
    public class Functions : AuthorizedFunctionBase
    {
    private readonly IService _loginService;

    public Functions(IService loginService, JwtSettings jwtSettings) : base(jwtSettings)
    {
        _loginService = loginService;
    }

    // Este endpoint debe permanecer público (sin autenticación)
    [LambdaFunction]
    [HttpApi(LambdaHttpMethod.Post, "/auth/login")]
    public async Task<APIGatewayHttpApiV2ProxyResponse> GetToken([FromBody] LoginRequest loginRequest, APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"Intento de login para usuario: {loginRequest.UserId}");

        try
        {
            var result = await _loginService.LoginAsync(loginRequest.UserId, loginRequest.Password);

            if (result.IsSuccess)
            {
                context.Logger.LogInformation($"Login exitoso para el usuario: {loginRequest.UserId}");
                return CreateSuccessResponse(result.Value);
            }
            else
            {
                context.Logger.LogWarning($"Login fallido para el usuario: {loginRequest.UserId}. Motivo: {result.Error}");
                return CreateErrorResponse(400, result.Error);
            }
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error en login: {ex.Message}");
            return CreateErrorResponse(500, "Error procesando la solicitud");
        }
    }
}
}