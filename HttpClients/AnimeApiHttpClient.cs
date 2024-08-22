namespace dsbot.HttpClients;

public class AnimeApiHttpClient(HttpClient httpClient) : IAnimeHttpClient
{
    public HttpClient HttpClient { get; private set; } = httpClient;
}
