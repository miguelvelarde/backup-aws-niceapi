using NiceAppApi.Utils.Auth;

namespace NiceAppApi.Auth;

public class LoginResult
{
    public bool Success { get; set; }
    public string Token { get; set; }
    public DateTime Expiry { get; set; }
    public string Message { get; set; } = "Have a nice day!";
}