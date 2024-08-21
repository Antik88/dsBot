using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;
using System.Diagnostics;

namespace dsbot.Commands
{
    public class PlayCommand : BaseCommandModule
    {
        [Command("play")]
        public async Task Play(CommandContext context, [RemainingText] string videoUrl)
        {
            var channel = context.Member.VoiceState?.Channel;

            var connection = await channel.ConnectAsync();
            var vnext = context.Client.GetVoiceNext();
            var transmit = connection.GetTransmitSink();

            var audioStreamUrl = await GetAudioStreamUrl(videoUrl);

            await PlayAudio(audioStreamUrl, transmit);
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
            return output;
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

            int retryCount = 0;
            const int maxRetries = 3;

            try
            {
                while (retryCount < maxRetries)
                {
                    process.Start();

                    _ = Task.Run(async () =>
                    {
                        string errorOutput = await process.StandardError.ReadToEndAsync();
                        if (!string.IsNullOrEmpty(errorOutput))
                        {
                            Console.WriteLine($"FFmpeg error: {errorOutput}");
                            throw new Exception(errorOutput);
                        }
                    });

                    try
                    {
                        await process.StandardOutput.BaseStream.CopyToAsync(transmit);
                        await process.WaitForExitAsync();

                        if (process.ExitCode != 0)
                        {
                            throw new Exception("FFmpeg exited with a non-zero code.");
                        }

                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error during audio playback: {ex.Message}");
                        retryCount++;

                        if (retryCount >= maxRetries)
                        {
                            throw new Exception("Maximum retry limit reached, unable to continue playback.");
                        }

                        Console.WriteLine("Retrying playback...");
                    }
                }
            }
            finally
            {
                await transmit.FlushAsync();
            }
        }
    }
}
