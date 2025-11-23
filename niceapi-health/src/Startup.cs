using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NiceAppApi.HealthCheck;

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

        // Register the health check service
        services.AddTransient<IService, Service>();
    }
}