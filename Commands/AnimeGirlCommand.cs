using dsbot.HttpClients;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json.Linq;
using System.Text.Json.Serialization;

namespace dsbot.Commands;

public class AnimeGirlCommand(IExternalServiceRequests<IAnimeHttpClient> animeService) : BaseCommandModule
{
    [Command("girl")]
    public async Task Test(CommandContext context)
    {
        //using HttpClient client = new();
        //var response = await client.GetAsync("https://api.waifu.pics/sfw/waifu");

        //var content = await response.Content.ReadAsStringAsync();

        var content = await animeService.GetFromService<JObject>("");

        //var json = JObject.Parse(content);
        var imageUrl = content["url"]?.ToString();

        await context.Channel.SendMessageAsync(imageUrl);
    }
}