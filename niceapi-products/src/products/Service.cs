using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Microsoft.Extensions.Configuration;

namespace NiceAppApi.Products
{
    public class Service : IService
    {
        private readonly IDynamoDBContext _context;

        public Service(IDynamoDBContext dynamoDb)
        {
            _context = dynamoDb;
        }

        public async Task<Result<IEnumerable<ProductModel>>> GetProductsAsync(ProductModel model)
        {
            try
            {
                var conditions = new List<ScanCondition>();

                if (!string.IsNullOrEmpty(model.IdProduct))
                    conditions.Add(new ScanCondition("IdProduct", ScanOperator.Equal, model.IdProduct));

                if (!string.IsNullOrEmpty(model.Name))
                    conditions.Add(new ScanCondition("Name", ScanOperator.Equal, model.Name));

                if (!string.IsNullOrEmpty(model.Description))
                    conditions.Add(new ScanCondition("Description", ScanOperator.Equal, model.Description));

                if (!string.IsNullOrEmpty(model.Type))
                    conditions.Add(new ScanCondition("Type", ScanOperator.Equal, model.Type));

                var products = await _context.ScanAsync<ProductModel>(conditions).GetRemainingAsync();

                return Result<IEnumerable<ProductModel>>.Success(products);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<ProductModel>>.Failure(ex.Message);
            }
        }

        public async Task<Result<ProductModel>> CreateProductAsync(ProductModel product, int userId)
        {
            try
            {
                ValidateProduct(product);

                if (string.IsNullOrEmpty(product.IdProduct))
                    product.IdProduct = Guid.NewGuid().ToString();

                await _context.SaveAsync(product);
                return Result<ProductModel>.Success(product);
            }
            catch (Exception ex)
            {
                return Result<ProductModel>.Failure(ex.Message);
            }
        }

        public async Task<Result<ProductModel>> UpdateProductAsync(ProductModel product, int userId)
        {
            try
            {
                ValidateProduct(product);
                await _context.SaveAsync(product);
                return Result<ProductModel>.Success(product);
            }
            catch (Exception ex)
            {
                return Result<ProductModel>.Failure(ex.Message);
            }
        }

        private static void ValidateProduct(ProductModel product)
        {
            if (string.IsNullOrEmpty(product.Name))
                throw new ArgumentException("El nombre del producto no puede estar vacío");

            if (string.IsNullOrEmpty(product.Description))
                throw new ArgumentException("La descripción del producto no puede estar vacía");

            if (string.IsNullOrEmpty(product.Image))
                throw new ArgumentException("La imagen del producto no puede estar vacía");

            if (product.Price <= 0)
                throw new ArgumentException("El precio debe ser mayor que cero");

            if (string.IsNullOrEmpty(product.Type))
                throw new ArgumentException("El tipo de producto no puede estar vacío");
        }
    }
}