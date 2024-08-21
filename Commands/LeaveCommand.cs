using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;

namespace dsbot.Commands;

public class LeaveCommand : BaseCommandModule
{
    [Command("leave")]
    public async Task Leave(CommandContext context) 
    {
        var vnext = context.Client.GetVoiceNext();
        var connection = vnext.GetConnection(context.Guild);

        connection.Disconnect();
    }
}
