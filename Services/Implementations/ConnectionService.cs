using dsbot.Services.Interfaces;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;

namespace dsbot.Services.Implementations;

public class ConnectionService : IConnectionService
{
    public async Task<VoiceNextConnection?> ConnectToChannel(CommandContext context)
    {
        var channel = context.Member.VoiceState?.Channel;

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
