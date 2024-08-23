using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using YoutubeSearchApi.Net.Models.Youtube;

namespace dsbot.Services.Interfaces;

public interface IAudioService
{
    void EnqueueTrack(CommandContext context, YoutubeVideo video);
    void SetTransmitSink(VoiceTransmitSink transmit);
    int GetQueueCount();
    Task Skip();
    Task Pause();
    Task Resume();
}