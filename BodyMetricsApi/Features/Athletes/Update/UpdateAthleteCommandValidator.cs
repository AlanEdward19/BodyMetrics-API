using BodyMetricsApi.Features.Athletes.Shared.Validators;
using FluentValidation;

namespace BodyMetricsApi.Features.Athletes.Update;

public sealed class UpdateAthleteCommandValidator : AthleteWriteCommandValidator<UpdateAthleteCommand>
{
    public UpdateAthleteCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
    }
}

