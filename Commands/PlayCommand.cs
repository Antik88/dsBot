using dsbot.Services.Interfaces;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using YoutubeSearchApi.Net.Services;

namespace dsbot.Commands
{
    public class PlayCommand(IConnectionService connectionService,
        IFeedBack feedBack, 
        IAudioService audioService) : BaseCommandModule
    {
        [Command("play")]
        public async Task Play(CommandContext context, [RemainingText] string query)
        {
            var connection = await connectionService.ConnectToChannel(context);

            var transmit = connection?.GetTransmitSink();
            if (transmit != null)
            {
                audioService.SetTransmitSink(transmit);
            }

            using (var httpClient = new HttpClient())
            {
                YoutubeSearchClient client = new(httpClient);

                var responseObject = await client.SearchAsync(query);

                var video = responseObject.Results.First();

                audioService.EnqueueTrack(context, video);

                await feedBack.QueuePosition(context, video, audioService.GetQueueCount());
            }
        }
    }
}
