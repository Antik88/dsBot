namespace dsbot.HttpClients;

public interface IExternalServiceRequests<THttpClient>
{
    public Task<T> GetFromService<T>(string query);
}