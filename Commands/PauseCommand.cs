using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;

namespace dsbot.Commands;

public class PauseCommand : BaseCommandModule
{
    private static bool _isPaused = false;

    [Command("pause")]
    public async Task PauseTrack(CommandContext context)
    {
        var userVC = context.Member.VoiceState?.Channel;
        if (userVC == null)
        {
            await context.Channel.SendMessageAsync("You need to be in a voice channel.");
            return;
        }

        var lavalink = context.Client.GetLavalink();
        if (!lavalink.ConnectedNodes.Any())
        {
            await context.Channel.SendMessageAsync("Lavalink is not connected.");
            return;
        }

        var node = lavalink.ConnectedNodes.Values.First();
        var connection = node.GetGuildConnection(context.Member.VoiceState.Guild);
        if (connection == null)
        {
            await context.Channel.SendMessageAsync("Not connected to a voice channel.");
            return;
        }

        if (connection.CurrentState.CurrentTrack == null)
        {
            await context.Channel.SendMessageAsync("No track is currently playing.");
            return;
        }

        if (_isPaused)
        {
            await connection.ResumeAsync();
            _isPaused = false;
            await context.Channel.SendMessageAsync(embed: new DiscordEmbedBuilder
            {
                Color = DiscordColor.DarkGreen,
                Title = "track resumed"
            });
        }
        else
        {
            await connection.PauseAsync();
            _isPaused = true;
            await context.Channel.SendMessageAsync(embed: new DiscordEmbedBuilder
            {
                Color = DiscordColor.DarkGray,
                Title = "track paused"
            });
        }
    }
}
