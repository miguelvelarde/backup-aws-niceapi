using NiceApiStock.Models;
using NiceApiStock.Utils;
using NiceApp.Common.Models;

namespace NiceApiStock.Services;

public class Service : IService
{
    private readonly Repositories.IRepository _stockRepository;

    public Service(Repositories.IRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public async Task<Result<StockModel>> GetAsync(string stockId, string userId)
    {
        try
        {
            var stock = await _stockRepository.GetByIdAsync(stockId, userId);
            
            // evaluar esta condicion. tal vez sea mejor enviar un response vacio en lugar de failure
            if (stock == null)
            {
                return Result<StockModel>.Failure(StockMessages.NOT_FOUND);
            }

            return Result<StockModel>.Success(stock);
        }
        catch (Exception ex)
        {
            return Result<StockModel>.Failure(ex.Message);
        }
    }

    public async Task<Result<StockModel>> CreateAsync(StockModel stock)
    {
        try
        {
            ValidateModel(stock);

            var stockExists = await _stockRepository.GetByIdAsync(stock.Id, stock.UserId);

            if (stockExists != null)
            {
                return Result<StockModel>.Failure(StockMessages.ALREADY_EXISTS);
            }

            await _stockRepository.CreateAsync(stock);

            return Result<StockModel>.Success(stock);
        }
        catch (Exception ex)
        {
            return Result<StockModel>.Failure(ex.Message);
        }
    }

    public async Task<Result<StockModel>> UpdateAsync(StockModel stock)
    {
        try
        {
            ValidateModel(stock);

            await _stockRepository.UpdateAsync(stock);

            return Result<StockModel>.Success(stock);
        }
        catch (Exception ex)
        {
            return Result<StockModel>.Failure(ex.Message);
        }
    }

    private static void ValidateModel(StockModel stock)
    {
        if (stock == null)
            throw new ArgumentNullException(nameof(stock), StockMessages.EMPTY_PRODUCT);

        if (string.IsNullOrEmpty(stock.Name))
            throw new ArgumentException(StockMessages.EMPTY_NAME);

        if (string.IsNullOrEmpty(stock.Id))
            throw new ArgumentException(StockMessages.INVALID_ID);

        if (string.IsNullOrEmpty(stock.UserId))
            throw new ArgumentException(StockMessages.EMPTY_USER);

        if (string.IsNullOrEmpty(stock.Description))
            throw new ArgumentException(StockMessages.EMPTY_DESCRIPTION);

        if (stock.Price <= 0)
            throw new ArgumentException(StockMessages.INVALID_PRICE);
    }
}