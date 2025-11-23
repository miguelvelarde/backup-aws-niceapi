using Amazon.Lambda.APIGatewayEvents;

namespace NiceApiUsers.Users
{
    public class ResultHandler
    {
        public static APIGatewayHttpApiV2ProxyResponse CreateSuccessResponse(object result)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = 200,
                Body = System.Text.Json.JsonSerializer.Serialize(result),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }

        public static APIGatewayHttpApiV2ProxyResponse CreateErrorResponse(int statusCode, string message)
        {
            return new APIGatewayHttpApiV2ProxyResponse
            {
                StatusCode = statusCode,
                Body = System.Text.Json.JsonSerializer.Serialize(new { error = message }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}