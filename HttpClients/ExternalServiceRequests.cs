using System.Net.Http.Json;

namespace dsbot.HttpClients;

public class ExternalServiceRequests<THttpClient>
        : IExternalServiceRequests<THttpClient> where THttpClient : IHttpClient
{
    private readonly HttpClient _httpClient;

    public ExternalServiceRequests(THttpClient httpClient)
    {
        _httpClient = httpClient.HttpClient;
    }

    public async Task<T> GetFromService<T>(string query)
    {
        var response = await _httpClient.GetAsync(query);

        var result = await response.Content.ReadFromJsonAsync<T>();

        return result;
    }
}
