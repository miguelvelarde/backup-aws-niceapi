public class HealthStatus
{
    public bool IsHealthy { get; set; }
    public string ApiGatewayStatus { get; set; }
    public string DatabaseStatus { get; set; }
    public long ResponseTimeMs { get; set; }
    public string Message { get; set; }
}