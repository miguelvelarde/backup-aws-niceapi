namespace NiceAppApi.Products
{
    public interface IService
    {
        Task<Result<IEnumerable<ProductModel>>> GetProductsAsync(ProductModel model);
        Task<Result<ProductModel>> CreateProductAsync(ProductModel product, int userId);
        Task<Result<ProductModel>> UpdateProductAsync(ProductModel product, int userId);
    }
}