using FluentValidation;

namespace BodyMetricsApi.Features.AthleteGroups.AddMember;

public sealed class AddAthleteToGroupCommandValidator : AbstractValidator<AddAthleteToGroupCommand>
{
    public AddAthleteToGroupCommandValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty().WithMessage("GroupId is required.");
        RuleFor(x => x.AthleteId).NotEmpty().WithMessage("AthleteId is required.");
    }
}
