using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using dsbot.Services.Interfaces;

namespace dsbot.Commands;

public class LeaveCommand(IConnectionService connectionService) : BaseCommandModule
{
    [Command(Constants.Commands.Leave)]
    public Task Leave(CommandContext context)
    {
        return connectionService.DisconnectFromChannel(context);
    }
}
