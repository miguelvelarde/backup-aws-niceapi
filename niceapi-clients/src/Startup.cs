using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NiceAppApi.Clients;
using NiceAppApi.Utils.Auth;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace NiceAppApi;

[Amazon.Lambda.Annotations.LambdaStartup]
public class Startup
{
    /// <summary>
    /// Services for Lambda functions can be registered in the services dependency injection container in this method. 
    ///
    /// The services can be injected into the Lambda function through the containing type's constructor or as a
    /// parameter in the Lambda function using the FromService attribute. Services injected for the constructor have
    /// the lifetime of the Lambda compute container. Services injected as parameters are created within the scope
    /// of the function invocation.
    /// </summary>
    public void ConfigureServices(IServiceCollection services)
    {
        
        // Create configuration from environment variables
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();
        
        services.AddSingleton<IConfiguration>(configuration);
        // Add the client service
        services.AddScoped<IService, Service>();
        
        // Add JWT configuration (ideally these values should be taken from environment variables)
        var jwtSecretKey = configuration["JWT_SECRET_KEY"] ?? "NiceApp-SuperSecretKey-12345!@#$%";
        var jwtExpiryMinutes = int.TryParse(configuration["JWT_EXPIRY_MINUTES"], out int mins) ? mins : 120;
        
        var jwtSettings = new JwtSettings
        {
            SecretKey = jwtSecretKey,
            ExpiryMinutes = jwtExpiryMinutes,
            Issuer = "NiceAppApi",
            Audience = "NiceAppMobile"
        };
        
        services.AddSingleton(jwtSettings);
        services.AddTransient<AuthMiddleware>();
    }
}
