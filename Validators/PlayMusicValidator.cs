using dsbot.Commands;
using DSharpPlus.Lavalink;
using DSharpPlus;
using FluentValidation;

namespace dsbot.Validators;

public class PlayMusicValidator : AbstractValidator<PlayMusicRequest>
{
    public PlayMusicValidator()
    {
        RuleFor(x => x.Context.Member.VoiceState.Channel)
            .NotNull().WithMessage("You must be in a voice channel");

        RuleFor(x => x.Context.Member.VoiceState)
            .NotNull().WithMessage("VoiceState cannot be null");

        RuleFor(x => x.Context.Client.GetLavalink().ConnectedNodes)
            .Must(nodes => nodes.Any()).WithMessage("No connection to Lavalink nodes");

        RuleFor(x => x.Context.Member.VoiceState.Channel.Type)
            .Equal(ChannelType.Voice).WithMessage("You must be in a voice channel");

        RuleFor(x => x.Query)
            .NotEmpty().WithMessage("Query cannot be empty");
    }
}
