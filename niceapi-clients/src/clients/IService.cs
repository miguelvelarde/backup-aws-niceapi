namespace NiceAppApi.Clients
{
    public interface IService
    {
        Task<Result<IEnumerable<ClientModel>>> GetClientsAsync(int userId, int? clientId, string name, string phone);
        Task<Result<ClientModel>> CreateClientAsync(ClientModel client);
        Task<Result<ClientModel>> UpdateClientAsync(ClientModel client);
    }
}