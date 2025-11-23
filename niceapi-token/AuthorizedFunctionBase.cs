using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Security.Claims;

namespace NiceAppApi.Utils.Auth;

public abstract class AuthorizedFunctionBase
{
    protected readonly AuthMiddleware _authMiddleware;

    public AuthorizedFunctionBase(JwtSettings jwtSettings)
    {
        _authMiddleware = new AuthMiddleware(jwtSettings);
    }

    protected ClaimsPrincipal ValidateRequest(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        var authHeader = request.Headers
        .FirstOrDefault(h => string.Equals(h.Key, "Authorization", StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrWhiteSpace(authHeader.Value))
        {
            throw new UnauthorizedAccessException("Acceso denegado: token no proporcionado");
        }

        return _authMiddleware.ValidateToken(authHeader.Value, context);
    }

    protected APIGatewayHttpApiV2ProxyResponse CreateSuccessResponse<T>(T data)
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Body = System.Text.Json.JsonSerializer.Serialize(data),
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }

    protected APIGatewayHttpApiV2ProxyResponse CreateErrorResponse(int statusCode, string message)
    {
        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = statusCode,
            Body = System.Text.Json.JsonSerializer.Serialize(new { message }),
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }

    protected string GetUserIdFromRequest(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        ClaimsPrincipal claims = ValidateRequest(request, context);

        foreach (var claim in claims.Claims)
        {
            context.Logger.LogInformation($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
        }

        string userId = claims.FindFirst("UserId")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException("No se pudo obtener el ID de usuario del token");
        }

        return userId;
    }
}