using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;

namespace dsBot.Helpers;

public static class FeedBackMusic
{
    public static async Task SendNowPlayTrack(CommandContext context,
        LavalinkTrack musicTrack)
    {
        string trackDilates = $"Now playing: {musicTrack.Title} \n" +
            $"Url: {musicTrack.Uri} \n" +
            $"Length: {musicTrack.Length}";

        var nowPlaying = new DiscordEmbedBuilder()
        {
            Color = DiscordColor.Purple,
            Title = $"joined channel {context.Member.VoiceState.Channel.Name}",
            Description = trackDilates
        };

        await context.Channel.SendMessageAsync(embed: nowPlaying);
    }
}
