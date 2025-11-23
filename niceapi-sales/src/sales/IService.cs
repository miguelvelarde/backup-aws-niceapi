namespace NiceAppApi.Sales
{
    public interface IService
    {
        Task<Result<IEnumerable<SaleDetailModel>>> GetSalesByUserIdAsync(int userId, int records);
        Task<Result<IEnumerable<SaleDetailModel>>> GetSalesByClientIdAsync(int clientId, int userId, int records);
        Task<Result<SaleDetailModel>> GetSaleByIdAsync(int saleId, int userId);
        Task<Result<SaleModel>> CreateSaleAsync(SaleModel sale);
        Task<Result<SaleModel>> UpdateSaleAsync(SaleModel sale);
    }
}