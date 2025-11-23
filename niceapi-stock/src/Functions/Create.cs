using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using NiceApiStock.Services;
using NiceApiStock.Models;
using NiceApiStock.Utils;
using System.Text.Json;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace NiceApiStock.Functions;

public class Create
{
    private static readonly IAmazonDynamoDB _dynamoDbClient = new AmazonDynamoDBClient();
    private static readonly IDynamoDBContext _dynamoDbContext = new DynamoDBContextBuilder()
        .WithDynamoDBClient(() => _dynamoDbClient)
        .Build();
    private readonly IService _stockService;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Create()
    {
        var stockRepository = new Repositories.DynamoDbRepository(_dynamoDbContext);
        _stockService = new Service(stockRepository);
    }

    [LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]
    public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        try
        {
            var stock = JsonSerializer.Deserialize<StockModel>(request.Body, JsonOptions);
            if (stock == null)
            {
                return ResultHandler.CreateErrorResponse(400, StockMessages.INVALID_DATA);
            }

            context.Logger.LogInformation($"Creating new stock: {JsonSerializer.Serialize(stock)}");

            var result = await _stockService.CreateAsync(stock);
            if (!result.IsSuccess)
            {
                return ResultHandler.CreateErrorResponse(400, result.Error ?? StockMessages.FAILED_CREATE);
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