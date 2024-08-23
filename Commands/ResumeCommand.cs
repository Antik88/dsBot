using dsbot.Services.Interfaces;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace dsbot.Commands;

public class ResumeCommand(IAudioService audioService, IFeedBack feedBack) : BaseCommandModule
{
    [Command("resume")]
    public Task Resume(CommandContext context)
    {
        feedBack.ResumePlaying(context);

        return audioService.Resume();
    }
}
