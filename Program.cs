using dsbot.Commands;
using dsbot.Constants;
using dsbot.Helpers;
using dsbot.HttpClients;
using dsbot.Services.Implementations;
using dsbot.Services.Interfaces;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace dsbot;

class Program
{
    private static DiscordClient? Client { get; set; }
    private static CommandsNextExtension? Commands { get; set; }
    static async Task Main(string[] args)
    {
        IHost _host = Host.CreateDefaultBuilder().ConfigureServices(
            services =>
            {
                services.AddSingleton<IApplication, Application>();
                services.AddSingleton<IAudioService, AudioService>();

                services.AddScoped<IConnectionService, ConnectionService>();
                services.AddScoped<IFeedBack, FeedBack>();
                services.AddScoped(typeof(IExternalServiceRequests<>), typeof(ExternalServiceRequests<>));

                services.AddHttpClient<IAnimeHttpClient, AnimeApiHttpClient>(client =>
                {
                    client.BaseAddress = new Uri(BaseUrl.AnimeApi);
                });
            })
            .Build();

        var app = _host.Services.GetRequiredService<IApplication>();

        var settings = new Settings();
        await settings.ReadSettings();

        var dsConfig = new DiscordConfiguration()
        {
            Intents = DiscordIntents.All,
            Token = settings.Token,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
        };

        Client = new DiscordClient(dsConfig);

        Client.Ready += ClientReady;

        Client.UseVoiceNext();

        var commandsConfig = new CommandsNextConfiguration()
        {
            StringPrefixes = [settings.Prefix],
            EnableMentionPrefix = true,
            EnableDms = true,
            EnableDefaultHelp = false,
            Services = _host.Services
        };

        Commands = Client.UseCommandsNext(commandsConfig);

        Commands.RegisterCommands<AnimeGirlCommand>();
        Commands.RegisterCommands<HelloCommand>();
        Commands.RegisterCommands<LeaveCommand>();
        Commands.RegisterCommands<PlayCommand>();

        await Client.ConnectAsync();
        await Task.Delay(-1);

        app.Run();
    }

    private static Task ClientReady(DiscordClient sender, ReadyEventArgs args) 
    {
        return Task.CompletedTask;
    }
}
