using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;

namespace dsbot.Commands;

public class LeaveCommand : BaseCommandModule
{
    [Command("leave")]
    public async Task Leave(CommandContext ctx)
    {
        var vnext = ctx.Client.GetVoiceNext();
        var connection = vnext.GetConnection(ctx.Guild);

        if (connection != null)
        {
            connection.Disconnect();
            await ctx.RespondAsync("Disconnected from the voice channel.");
        }
        else
        {
            await ctx.RespondAsync("I'm not connected to a voice channel.");
        }
    }

}
