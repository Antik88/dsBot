using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json.Linq;

namespace dsbot.Commands;

public class AnimeGrilCommand : BaseCommandModule
{
    [Command("girl")]
    public async Task Test(CommandContext context)
    {
        using HttpClient client = new();
        var response = await client.GetAsync("https://api.waifu.pics/sfw/waifu");

        var content = await response.Content.ReadAsStringAsync();

        var json = JObject.Parse(content);
        var imageUrl = json["url"]?.ToString();

        await context.Channel.SendMessageAsync(imageUrl);
    }
}