using dsbot.Helpers;
using dsbot.Services.Interfaces;
using dsbot.Validators;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Lavalink;

namespace dsbot.Commands;

public class MusicCommand(IFeedBack feedBack) : BaseCommandModule
{
    [Command("track")]
    public async Task PlayMusic(CommandContext context,
        [RemainingText] string query)
    {
        var request = new PlayMusicRequest
        {
            Context = context,
            Query = query
        };

        var validator = new PlayMusicValidator();
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                await context.Channel.SendMessageAsync(error.ErrorMessage);
            }
            return;
        }

        var userVC = context.Member.VoiceState.Channel;
        var lavalickInst = context.Client.GetLavalink();

        var node = lavalickInst.ConnectedNodes.Values.First();
        await node.ConnectAsync(userVC);

        var connection = node.GetGuildConnection(context.Member.VoiceState.Guild);
        if (connection == null)
        {
            await context.Channel.SendMessageAsync("-_-");
            return;
        }

        var searchResult = await node.Rest.GetTracksAsync(query);

        if (searchResult.LoadResultType == LavalinkLoadResultType.NoMatches
            || searchResult.LoadResultType == LavalinkLoadResultType.LoadFailed)
        {
            await context.Channel.SendMessageAsync($"no found {query}");
            return;
        }

        var musicTrack = searchResult.Tracks.First();

        await connection.PlayAsync(musicTrack);

        await feedBack.SendNowPlayTrack(context, musicTrack);
    }
}
