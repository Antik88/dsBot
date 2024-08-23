using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;

namespace dsbot.Services.Interfaces;

public interface IConnectionService
{
    public Task<VoiceNextConnection?> ConnectToChannel(CommandContext context);
    public Task DisconnectFromChannel(CommandContext context);
}