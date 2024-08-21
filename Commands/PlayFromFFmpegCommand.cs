using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System.Diagnostics;

namespace dsbot.Commands;


public class PlayFromFFmpegCommand : BaseCommandModule
{
    [Command("play")]
    public async Task PlayCommand(CommandContext context, [RemainingText] string path)
    {
        var channel = context.Member.VoiceState?.Channel;
        await channel.ConnectAsync();

        var vnext = context.Client.GetVoiceNext();
        var connection = vnext.GetConnection(context.Guild);

        var transmit = connection.GetTransmitSink();

        var pcm = ConvertAudioToPcm(path);
        await pcm.CopyToAsync(transmit);
        await pcm.DisposeAsync();
    }

    [Command("leave")]
    public async Task LeaveCommand(CommandContext ctx)
    {
        var vnext = ctx.Client.GetVoiceNext();
        var connection = vnext.GetConnection(ctx.Guild);

        connection.Disconnect();
    }

    private Stream ConvertAudioToPcm(string filePath)
    {
        var ffmpeg = Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $@"-i ""{filePath}"" -ac 2 -f s16le -ar 48000 pipe:1",
            RedirectStandardOutput = true,
            UseShellExecute = false
        });

        return ffmpeg.StandardOutput.BaseStream;
    }
}
