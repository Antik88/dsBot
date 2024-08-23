using dsbot.Services.Interfaces;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace dsbot.Commands;

public class PauseCommand(IAudioService audioService, IFeedBack feedBack) : BaseCommandModule 
{
    [Command("pause")]
    public Task Pause(CommandContext context)
    {
        feedBack.PausePlaying(context);

        return audioService.Pause();
    }
}
