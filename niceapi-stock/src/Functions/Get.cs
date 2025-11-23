using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using NiceApiStock.Services;
using NiceApiStock.Repositories;
using NiceApiStock.Utils;

namespace NiceApiStock.Functions;

public class Get
{
    private static readonly IAmazonDynamoDB _dynamoDbClient = new AmazonDynamoDBClient();
    private static readonly IDynamoDBContext _dynamoDbContext = new DynamoDBContextBuilder()
        .WithDynamoDBClient(() => _dynamoDbClient)
        .Build();
    private readonly IService _service;

    public Get()
    {
        var stockRepository = new DynamoDbRepository(_dynamoDbContext);
        _service = new Service(stockRepository);
    }

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        string id = "";
        string userId = "";

        if (request.QueryStringParameters != null)
        {
            if (!request.QueryStringParameters.ContainsKey("id") || 
                !request.QueryStringParameters.ContainsKey("userId"))
            {
                return ResultHandler.CreateErrorResponse(400, "Both 'id' and 'userId' parameters are required");
            }

            id = request.QueryStringParameters["id"];
            userId = request.QueryStringParameters["userId"];

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(userId))
            {
                return ResultHandler.CreateErrorResponse(400, "Parameters 'id' and 'userId' cannot be empty");
            }
        }
        else
        {
            return ResultHandler.CreateErrorResponse(400, "Query parameters are required");
        }

        context.Logger.LogInformation($"Getting stock with id: {id} and user: {userId}");

        try
        {
            var result = await _service.GetAsync(id, userId);

            if (!result.IsSuccess)
            {
                return ResultHandler.CreateErrorResponse(404, result.Error ?? StockMessages.NOT_FOUND);
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
            return ResultHandler.CreateErrorResponse(500, StockMessages.INTERNAL_SERVER_ERROR);
        }
    }
}