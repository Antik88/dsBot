using dsbot.Services.Interfaces;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

namespace dsbot.Services.Implementations;

public class ConnectionService : IConnectionService
{
    public async Task<VoiceNextConnection?> ConnectToChannel(CommandContext context)
    {
        var channel = context.Member.VoiceState?.Channel;

        return channel == null
            ? await HandleNoChannel(context)
            : await ConnectToValidChannel(context, channel);
    }

    private async Task<VoiceNextConnection?> HandleNoChannel(CommandContext context)
    {
        await context.RespondAsync("You need to be in a voice channel first!");
        return null;
    }

    private async Task<VoiceNextConnection?> ConnectToValidChannel(CommandContext context, DiscordChannel channel)
    {
        var voiceNext = context.Client.GetVoiceNext();
        var connection = voiceNext.GetConnection(context.Guild);

        connection ??= await channel.ConnectAsync();
        return connection;
    }

    public async Task DisconnectFromChannel(CommandContext context)
    {
        var vnext = context.Client.GetVoiceNext();
        var connection = vnext.GetConnection(context.Guild);

        if (connection != null)
        {
            connection.Disconnect();
            await context.RespondAsync("Disconnected from the voice channel.");
        }
        else
        {
            await context.RespondAsync("I'm not connected to a voice channel.");
        }
    }
}
