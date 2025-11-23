namespace NiceAppApi.Auth;

// Modelo para solicitudes de login
public class LoginRequest
{
    public int UserId { get; set; }
    public string Password { get; set; }
}