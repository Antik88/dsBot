using DSharpPlus.CommandsNext;

namespace dsbot.Helpers;

public class PlayMusicRequest
{
    public CommandContext Context { get; set; }
    public string Query { get; set; } = string.Empty;
}
