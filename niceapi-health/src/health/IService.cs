namespace NiceAppApi.HealthCheck
{
    public interface IService
    {
        Task<HealthStatus> CheckHealthAsync();
    }
}