using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace NiceAppApi.Utils.Auth
{
    public static class Tokenizer
    {        
        // Generar token JWT
        public static string GenerateJwtToken(UserModel user, int tokenExpiryMinutes)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("NiceApp-SuperSecretKey-12345!@#$%"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.UserId),
                new Claim("Role", user.Type)
            };

            var token = new JwtSecurityToken(
                issuer: "NiceAppApi",
                audience: "NiceAppMobile",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(tokenExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Método para hash de contraseñas
        public static string HashPassword(string password)
        {
            var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        // Método para verificar contraseña
        public static bool VerifyPassword(string password, string storedHash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == storedHash;
        }

        public static string GetHashedValue(string value)
        {
            return HashPassword(value);
        }

    }
}