using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Amazon.Lambda.Core;
using Microsoft.IdentityModel.Tokens;

namespace NiceAppApi.Utils.Auth
{
    public class AuthMiddleware
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public AuthMiddleware(JwtSettings settings)
        {
            _secretKey = settings.SecretKey;
            _issuer = settings.Issuer;
            _audience = settings.Audience;
        }

        public ClaimsPrincipal ValidateToken(string authHeader, ILambdaContext context)
        {
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Logger.LogError("Token no proporcionado o en formato incorrecto");
                throw new UnauthorizedAccessException("Acceso denegado: token inválido");
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);
                
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                
                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Error validando token: {ex.Message}");
                throw new UnauthorizedAccessException("Acceso denegado: token inválido");
            }
        }        // Add JWT configuration (ideally these values should be taken from environment variables)
    }
}