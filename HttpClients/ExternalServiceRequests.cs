using System.Net.Http.Json;

namespace dsbot.HttpClients;

public class ExternalServiceRequests<THttpClient>(THttpClient httpClient)
        : IExternalServiceRequests<THttpClient> where THttpClient : IHttpClient
{
    private readonly HttpClient _httpClient = httpClient.HttpClient;

    public async Task<T> GetFromService<T>(string query)
    {
        var response = await _httpClient.GetAsync(query);

        if (!response.IsSuccessStatusCode)
            throw new Exception(response.StatusCode.ToString());

        var result = await response.Content.ReadFromJsonAsync<T>()
            ?? throw new Exception("result is null");

        return result;
    }
}
