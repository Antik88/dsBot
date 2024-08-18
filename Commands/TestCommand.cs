using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace dsbot.Commands;

public class TestCommands : BaseCommandModule
{
    [Command("hello")]
    public async Task Test(CommandContext context)
    {
        await context.Channel
            .SendMessageAsync($"Hello {context.User.Username}");
    }
}