using BodyMetricsApi.Features.Sports.Shared.Interfaces;
using BodyMetricsApi.Features.Sports.Shared.ViewModels;
using BodyMetricsApi.Shared.Results;
using BodyMetricsApi.Shared.Validation;
using FluentValidation;

namespace BodyMetricsApi.Features.Sports.Create;

public sealed class CreateSportCommandHandler(ISportRepository repository, IValidator<CreateSportCommand> validator)
{
    public async Task<OperationResult<SportResponse>> HandleAsync(CreateSportCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return OperationResult<SportResponse>.Validation(validationResult.ToErrorDictionary());
        }

        var sport = Sport.Create(command.Name, command.Sectors, command.Categories);
        await repository.AddAsync(sport, cancellationToken);
        return OperationResult<SportResponse>.Success(sport.ToResponse(), StatusCodes.Status201Created);
    }
}

