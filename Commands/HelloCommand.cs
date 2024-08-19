using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace dsbot.Commands;

public class TestCommands : BaseCommandModule
{
    [Command("hello")]

    public Task SayHello(CommandContext context)
    {
        return context.Channel
            .SendMessageAsync($"Hello {context.User.Username}");
    }
}