using DSharpPlus.CommandsNext;
using DSharpPlus.Lavalink;

namespace dsbot.Services.Interfaces;

public interface IFeedBack
{
    public Task SendNowPlayTrack(CommandContext context, LavalinkTrack musicTrack);
}
