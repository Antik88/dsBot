using dsbot.Services.Interfaces;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using YoutubeSearchApi.Net.Models.Youtube;

namespace dsbot.Services.Implementations;

public class FeedBack : IFeedBack
{
    public Task QueuePosition(CommandContext context, YoutubeVideo video, int position)
    {
        string positionDilates = $"Track: {video.Title} added to queue";

        var addToQueue = new DiscordEmbedBuilder()
        {
            Color = DiscordColor.Purple,
            Title = $"Position #{position}",
            Description = positionDilates 
        };

        return context.Channel.SendMessageAsync(embed: addToQueue);
    }

    public Task NowPlaying(CommandContext context, YoutubeVideo video)
    {
        string trackDilates = $"Url: {video.Url} \n" +
            $"Author: {video.Author} \n" +
            $"Duration: {video.Duration}";

        var nowPlaying = new DiscordEmbedBuilder()
        {
            Color = DiscordColor.Purple,
            Title = $"Now playing {video.Title}",
            Description = trackDilates
        };

        return context.Channel.SendMessageAsync(embed: nowPlaying);
    }

    public Task PausePlaying(CommandContext context)
    {
        return context.Channel.SendMessageAsync(embed: new DiscordEmbedBuilder()
        {
            Color = DiscordColor.DarkBlue,
            Title = "Paused"
        });
    }

    public Task ResumePlaying(CommandContext context)
    {
        return context.Channel.SendMessageAsync(embed: new DiscordEmbedBuilder()
        {
            Color = DiscordColor.DarkGreen,
            Title = "Resume"
        });
    }

    public Task Skip(CommandContext context, YoutubeVideo currentPlay)
    {
        return context.Channel.SendMessageAsync(embed: new DiscordEmbedBuilder()
        {
            Color = DiscordColor.DarkGray,
            Title = $"{currentPlay.Title} skipped."
        });
    }
}
