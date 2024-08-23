using dsbot.Constants;
using dsbot.Services.Interfaces;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using System.Diagnostics;
using YoutubeSearchApi.Net.Models.Youtube;

namespace dsbot.Services.Implementations;

public class AudioService(IFeedBack feedBack) : IAudioService
{
    private readonly Queue<YoutubeVideo> _playQueue = new();
    private bool _isPlaying = false;
    private VoiceTransmitSink _transmit;

    public void SetTransmitSink(VoiceTransmitSink transmit)
    {
        _transmit = transmit;
    }

    public void EnqueueTrack(CommandContext context, YoutubeVideo video)
    {
        _playQueue.Enqueue(video);
        if (!_isPlaying && _transmit != null)
        {
            _isPlaying = true;
            Task.Run(() => PlayNextTrack(context));
        }
    }

    public async Task PlayNextTrack(CommandContext context)
    {
        if (_playQueue.Count == 0)
        {
            _isPlaying = false;
            return;
        }

        var video = _playQueue.Dequeue();

        await feedBack.NowPlaying(context, video);

        var audioStreamUrl = await GetAudioStreamUrl(video.Url);

        await PlayAudio(audioStreamUrl, _transmit);

        await PlayNextTrack(context);
    }

    private async Task<string> GetAudioStreamUrl(string videoUrl)
    {
        var ytDlpPath = ProgramsPath.ytDlpPath;
        var startInfo = new ProcessStartInfo
        {
            FileName = ytDlpPath,
            Arguments = string.Format(Arguments.YtDlpArgumentsTemplate, videoUrl),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        var output = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();
        return output.Trim();
    }

    private static async Task PlayAudio(string audioStreamUrl, VoiceTransmitSink transmit)
    {
        var ffmpegPath = ProgramsPath.ffmpeg;
        var startInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = string.Format(Arguments.FfmpegArgumentsTemplate, audioStreamUrl),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        using var process = new Process { StartInfo = startInfo };

        process.Start();

        _ = Task.Run(async () =>
        {
            string errorOutput = await process.StandardError.ReadToEndAsync();
            if (!string.IsNullOrEmpty(errorOutput))
            {
                throw new Exception(errorOutput);
            }
        });

        await process.StandardOutput.BaseStream.CopyToAsync(transmit);
        await process.WaitForExitAsync();
    }

    public int GetQueueCount()
    {
        return _playQueue.Count;
    }

    public async Task Pause()
    {
    }

    public async Task Resume()
    {
    }

    public async Task Skip()
    {
    }
}
