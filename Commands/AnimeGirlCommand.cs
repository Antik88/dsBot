using dsbot.HttpClients;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Text.Json.Nodes;

namespace dsbot.Commands;

public class AnimeGirlCommand(IExternalServiceRequests<IAnimeHttpClient> animeService) : BaseCommandModule
{
    [Command("girl")]
    public async Task Test(CommandContext context)
    {
        var content = await animeService.GetFromService<JsonObject>("");

        var imageUrl = content["url"]?.ToString();

        await context.Channel.SendMessageAsync(imageUrl);
    }
}