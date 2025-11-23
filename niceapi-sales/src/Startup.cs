using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Amazon.Lambda.Core;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using NiceAppApi.Utils.Auth;
using Amazon.Lambda.APIGatewayEvents;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;

namespace NiceAppApi.Sales;

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
        // Crear y registrar IConfiguration
        var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true)
                    .AddEnvironmentVariables(); // Importante para AWS Lambda

        var configuration = builder.Build();
        services.AddSingleton<IConfiguration>(configuration);

        // Registrar Service y JwtSettings
        services.AddSingleton<IService, Service>();
        
        // Configurar JwtSettings
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
