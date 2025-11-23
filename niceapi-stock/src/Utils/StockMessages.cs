namespace NiceApiStock.Utils;

public static class StockMessages
{
    public const string NOT_FOUND = "Stock not found";
    public const string INVALID_DATA = "Invalid stock data";
    public const string FAILED_CREATE = "Failed to create stock";
    public const string FAILED_UPDATE = "Failed to update stock";
    public const string INTERNAL_SERVER_ERROR = "Internal server error";
    public const string CREATED_OK = "Stock created successfully";
    public const string UPDATE_OK = "Stock updated successfully";
    public const string ALREADY_EXISTS = "Stock already exists";
    public const string EMPTY_PRODUCT = "Stock cannot be empty";
    public const string EMPTY_NAME = "Stock name cannot be empty";
    public const string EMPTY_DESCRIPTION = "Stock description cannot be empty";
    public const string EMPTY_PRICE = "Stock price cannot be empty";
    public const string INVALID_PRICE = "Stock price must be a valid number greater than zero";
    public const string INVALID_ID = "Invalid stock Id";
    public const string EMPTY_USER = "UserId cannot be empty";
    public const string MISSING_PARAMETERS = "Required parameters are missing";
    public const string UNAUTHORIZED_ACCESS = "Unauthorized access to this resource";
}
