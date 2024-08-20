using dsbot.Commands;
using dsbot.Services.Implementations;
using dsbot.Services.Interfaces;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
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
                services.AddScoped<IFeedBack, FeedBackMusic>();      
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

        Client.Ready += Client_Ready;   

        var commandsConfig = new CommandsNextConfiguration()
        {
            StringPrefixes = [settings.Prefix],
            EnableMentionPrefix = true,
            EnableDms = true,
            EnableDefaultHelp = false,
            Services = _host.Services
        };

        Commands = Client.UseCommandsNext(commandsConfig);

        Commands.RegisterCommands<TestCommands>();
        Commands.RegisterCommands<AnimeGrilCommand>();
        Commands.RegisterCommands<MusicCommand>();
        Commands.RegisterCommands<PauseCommand>();

        var endpoint = new ConnectionEndpoint
        {
            Hostname = "lava-v3.ajieblogs.eu.org",
            Port = 443,
            Secured = true,
        };

        var lavaLinkConfiguration = new LavalinkConfiguration()
        {
            Password = "https://dsc.gg/ajidevserver",
            RestEndpoint = endpoint,
            SocketEndpoint = endpoint,
        };

        var lavaLink = Client.UseLavalink();

        await Client.ConnectAsync();
        await lavaLink.ConnectAsync(lavaLinkConfiguration);
        await Task.Delay(-1);

        app.Run();
    }

    private static Task Client_Ready(DiscordClient sender, ReadyEventArgs args) 
    {
        return Task.CompletedTask;
    }
}
