using DSharpPlus.CommandsNext;
using YoutubeSearchApi.Net.Models.Youtube;

namespace dsbot.Services.Interfaces;

public interface IFeedBack
{
    Task QueuePosition(CommandContext context, YoutubeVideo video, int position);
    Task NowPlaying(CommandContext context, YoutubeVideo video);
}
