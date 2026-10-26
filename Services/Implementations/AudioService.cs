using dsbot.Constants;
using dsbot.Services.Interfaces;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using System.Diagnostics;
using System.Text;
using YoutubeSearchApi.Net.Models.Youtube;

namespace dsbot.Services.Implementations;

public class AudioService(IFeedBack feedBack) : IAudioService
{
    private readonly Queue<YoutubeVideo> _playQueue = new();
    private bool _isPlaying = false;
    private bool _isPaused = false;
    private VoiceTransmitSink? _transmit;
    private TaskCompletionSource<bool>? _pauseCompletionSource;
    private TaskCompletionSource<bool> _skipCompletionSource;
    private YoutubeVideo _currentTrack;
    private Process _ffmpegProcess;

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
        _currentTrack = video;

        var audioStreamUrl = await GetAudioStreamUrl(video.Url);

        _skipCompletionSource = new TaskCompletionSource<bool>();

        await PlayAudio(audioStreamUrl, _transmit);

        await PlayNextTrack(context);
    }

    private async Task<string> GetAudioStreamUrl(string videoUrl)
    {
        var ytDlpPath = ProgramsPath.ytDlpPath;
        var cookiesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cookies.txt");

        var startInfo = new ProcessStartInfo
        {
            FileName = ytDlpPath,
            Arguments = $"-f ba --geo-bypass --cookies \"{cookiesPath}\" --get-url \"{videoUrl}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        try
        {
            using var process = new Process { StartInfo = startInfo };
            process.Start();

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            if (process.ExitCode != 0 || string.IsNullOrWhiteSpace(output))
            {
                throw new Exception($"yt-dlp error: {error}");
            }

            return output.Trim();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting audio URL: {ex.Message}");
            throw;
        }
    }

    private async Task PlayAudio(string audioStreamUrl, VoiceTransmitSink transmit)
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

        _ffmpegProcess = new Process { StartInfo = startInfo };
        _ffmpegProcess.Start();

        _ = Task.Run(async () =>
        {
            string errorOutput = await _ffmpegProcess.StandardError.ReadToEndAsync();
            if (!string.IsNullOrEmpty(errorOutput))
            {
                throw new Exception(errorOutput);
            }
        });

        _pauseCompletionSource = new TaskCompletionSource<bool>();

        using var audioStream = _ffmpegProcess.StandardOutput.BaseStream;
        var buffer = new byte[4096];
        int bytesRead;

        while ((bytesRead = await audioStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            if (_isPaused)
            {
                await _pauseCompletionSource.Task;
            }

            if (_skipCompletionSource.Task.IsCompleted)
            {
                break;
            }

            await transmit.WriteAsync(buffer.AsMemory(0, bytesRead));
        }

        await _ffmpegProcess.WaitForExitAsync();
    }

    public int GetQueueCount()
    {
        return _playQueue.Count;
    }

    public Task Pause()
    {
        if (_isPlaying && !_isPaused)
        {
            _isPaused = true;
            _pauseCompletionSource = new TaskCompletionSource<bool>();
        }
        return Task.CompletedTask;
    }

    public Task Resume()
    {
        if (_isPlaying && _isPaused)
        {
            _isPaused = false;
            _pauseCompletionSource.TrySetResult(true);
        }
        return Task.CompletedTask;
    }

    public Task Skip()
    {
        if (_isPlaying)
        {
            _skipCompletionSource.TrySetResult(true);

            if (_ffmpegProcess != null && !_ffmpegProcess.HasExited)
            {
                _ffmpegProcess.Kill();
            }
        }
        return Task.CompletedTask;
    }

    public YoutubeVideo GetCurrentTrack()
    {
        return _currentTrack;
    }
}
