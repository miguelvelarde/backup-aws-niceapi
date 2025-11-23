using Amazon.Lambda.Core;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using NiceAppApi.Utils.Auth;
using Amazon.Lambda.APIGatewayEvents;
using System.Security.Claims;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NiceAppApi.Products
{
    public class Functions : AuthorizedFunctionBase
    {
        private readonly IService _productService;

        public Functions(IService productService, JwtSettings jwtSettings)
            : base(jwtSettings)
        {
            _productService = productService;
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Get, "/products")]
        public async Task<APIGatewayHttpApiV2ProxyResponse> GetProducts([FromBody] ProductModel product, APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            string query = $"Id: {product.IdProduct} Name: {product.Name} Desc: {product.Description} Type: {product.Type}";
            context.Logger.LogInformation($"Get Product: {query}");

            try
            {
                ClaimsPrincipal user = ValidateRequest(request, context);

                var serviceResult = await _productService.GetProductsAsync(product);
                if (!serviceResult.IsSuccess)
                {
                    return CreateErrorResponse(500, serviceResult.Error);
                }

                return CreateSuccessResponse(serviceResult.Value);
            }
            catch (UnauthorizedAccessException ex)
            {
                return CreateErrorResponse(401, ex.Message);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error: {ex.Message}");
                return CreateErrorResponse(500, "Error interno del servidor");
            }
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Post, "/products")]
        public async Task<APIGatewayHttpApiV2ProxyResponse> CreateProduct([FromBody] ProductModel product, APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogInformation($"Creating new product: {product.Name}");

            try
            {
                ClaimsPrincipal user = ValidateRequest(request, context);

                string userId = user.FindFirst("userId")?.Value;

                var result = await _productService.CreateProductAsync(product, Convert.ToInt32(userId));

                return CreateSuccessResponse(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return CreateErrorResponse(401, ex.Message);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error: {ex.Message}");
                return CreateErrorResponse(500, "Error interno del servidor");
            }
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Put, "/products")]
        public async Task<APIGatewayHttpApiV2ProxyResponse> UpdateProduct([FromBody] ProductModel product, APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogInformation($"Updating product with ID: {product.IdProduct}");

            try
            {
                ClaimsPrincipal user = ValidateRequest(request, context);
                string userId = user.FindFirst("userId")?.Value;

                var result = await _productService.UpdateProductAsync(product, Convert.ToInt32(userId));
                return CreateSuccessResponse(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return CreateErrorResponse(401, ex.Message);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error: {ex.Message}");
                return CreateErrorResponse(500, "Error interno del servidor");
            }
        }
    }
}