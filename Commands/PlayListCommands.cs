using dsbot.Commands;
using dsbot.Services.Interfaces;
using dsBot.DataContext;
using dsBot.DataContext.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using YoutubeSearchApi.Net.Services;

namespace dsBot.Commands;

public class PlayListCommands(IConnectionService connectionService,
    IFeedBack feedBack,
    IAudioService audioService) : BaseCommandModule
{
    [Command("show")]
    public async Task CreatePlayList(CommandContext context)
    {
        using var dbContext = new ApplicationDbContext();

        var playLists = await dbContext.PlayLists.ToListAsync();

        var description = playLists.Any()
            ? string.Join("\n", playLists.Select(pl => $"{pl.Id}: {pl.Name}"))
            : "play list not found";

        var embed = new DiscordEmbedBuilder()
        {
            Color = DiscordColor.Purple,
            Title = "PlayLists",
            Description = description
        };

        await context.Channel.SendMessageAsync(embed: embed.Build());
    }

    [Command("new_playlist")]

    public async Task CreatePlayList(CommandContext context, [RemainingText] string name)
    {
        using var dbContext = new ApplicationDbContext();

        var playList = new PlayList { Name = name };

        dbContext.PlayLists.Add(playList);

        dbContext.SaveChanges();
    }

    [Command("add_to")]
    public async Task AddToPlayList(CommandContext context, int playListId, [RemainingText] string trackName)
    {
        using var dbContext = new ApplicationDbContext();

        var playList = await dbContext.PlayLists.FindAsync(playListId);
        if (playList == null)
        {
            await context.RespondAsync("play list not found");
            return;
        }

        var track = new Tracks { Name = trackName, PlayListId = playListId };
        dbContext.Tracks.Add(track);
        await dbContext.SaveChangesAsync();

        await context.RespondAsync($"track \"{trackName}\" was added to playlist with Id {playListId}.");
    }

    [Command("playlist")]
    public async Task startPlayList(CommandContext context, [RemainingText] int playListId)
    {
        var connection = await connectionService.ConnectToChannel(context);

        var transmit = connection?.GetTransmitSink();
        if (transmit != null)
        {
            audioService.SetTransmitSink(transmit);
        }

        using var httpClient = new HttpClient();

        YoutubeSearchClient client = new(httpClient);

        using var dbContext = new ApplicationDbContext();

        var playList = await dbContext.Tracks
            .Where(t => t.PlayListId == playListId)
            .ToListAsync();

        if (playList == null)
        {
            await context.RespondAsync("play list not found");
            return;
        }

        var random = new Random();
        var shuffledTracks = playList.OrderBy(t => random.Next()).ToList();

        foreach (var track in shuffledTracks)
        {
            var responseObject = await client.SearchAsync(track.Name);
            var video = responseObject.Results.First();

            audioService.EnqueueTrack(context, video);

            await feedBack.QueuePosition(context, video, audioService.GetQueueCount());
        }
    }
}
