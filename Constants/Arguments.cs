namespace dsbot.Constants;

public static class Arguments
{
    public const string YtDlpArgumentsTemplate = "-f bestaudio --get-url \"{0}\"";
    public const string FfmpegArgumentsTemplate = "-reconnect 1 -reconnect_streamed 1 -reconnect_delay_max 5 -i \"{0}\" -ac 2 -f s16le -ar 48000 pipe:1";
}
