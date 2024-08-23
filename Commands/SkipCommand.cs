using dsbot.Services.Interfaces;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace dsbot.Commands;

public class SkipCommand(IAudioService audioService, IFeedBack feedBack) : BaseCommandModule
{
    [Command("skip")]
    public Task Skip(CommandContext context)
    {
        feedBack.Skip(context, audioService.GetCurrentTrack());

        return audioService.Skip();
    }
}
