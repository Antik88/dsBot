using dsbot.Services.Interfaces;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace dsbot.Commands;

public class HelpCommand(IFeedBack feedBack) : BaseCommandModule
{
    [Command(Constants.Commands.Help)]
    public Task Help(CommandContext context)
    {
        return feedBack.Help(context);
    }
}
