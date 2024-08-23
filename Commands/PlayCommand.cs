using dsbot.Constants;
using dsbot.Services.Interfaces;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;
using System.Diagnostics;
using YoutubeSearchApi.Net.Services;

namespace dsbot.Commands
{
    public class PlayCommand(IConnectionService connectionService) : BaseCommandModule
    {
        private readonly Queue<string> _playQueue = new();
        private bool _isPlaying = false;

        [Command("play")]
        public async Task Play(CommandContext context, [RemainingText] string query)
        {
            var connection = await connectionService.ConnectToChannel(context);

            var transmit = connection?.GetTransmitSink();

            using (var httpClient = new HttpClient())
            {
                YoutubeSearchClient client = new (httpClient);

                var responseObject = await client.SearchAsync(query);

                _playQueue.Enqueue(responseObject.Results.First().Url);
            }

            await context.Channel.SendMessageAsync($"Track added to queue \n Queue place #{_playQueue.Count}");

            if (!_isPlaying && transmit != null)
            {
                _isPlaying = true;
                await PlayNextTrack(transmit);
            }
        }

        private async Task PlayNextTrack(VoiceTransmitSink transmit)
        {
            if (_playQueue.Count == 0)
            {
                _isPlaying = false;
                return;
            }

            var videoUrl = _playQueue.Dequeue();
            var audioStreamUrl = await GetAudioStreamUrl(videoUrl);

            await PlayAudio(audioStreamUrl, transmit);

            await PlayNextTrack(transmit);
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
    }
}
