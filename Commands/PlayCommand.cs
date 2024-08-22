using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;
using System.Diagnostics;

namespace dsbot.Commands
{
    public class PlayCommand : BaseCommandModule
    {
        private readonly Queue<string> _playQueue = new();
        private bool _isPlaying = false;

        [Command("play")]
        public async Task Play(CommandContext context, [RemainingText] string videoUrl)
        {
            var channel = context.Member.VoiceState?.Channel;

            var voiceNext = context.Client.GetVoiceNext();
            var connection = voiceNext.GetConnection(context.Guild);

            connection ??= await channel.ConnectAsync();

            var transmit = connection.GetTransmitSink();

            _playQueue.Enqueue(videoUrl);
            await context.Channel.SendMessageAsync($"Track added to queue \n Queue place #{_playQueue.Count}");

            if (!_isPlaying)
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
            var ytDlpPath = "yt-dlp";
            var startInfo = new ProcessStartInfo
            {
                FileName = ytDlpPath,
                Arguments = $"-f bestaudio --get-url \"{videoUrl}\"",
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

        private async Task PlayAudio(string audioStreamUrl, VoiceTransmitSink transmit)
        {
            var ffmpegPath = "ffmpeg";
            var startInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = $"-reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 5 -i \"{audioStreamUrl}\" -ac 2 -f s16le -ar 48000 pipe:1",
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
