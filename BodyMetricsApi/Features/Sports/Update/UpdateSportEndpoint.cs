using BodyMetricsApi.Shared.Results;
using BodyMetricsApi.Shared.Validation;
using FluentValidation;

namespace BodyMetricsApi.Features.Sports.Update;

public sealed class UpdateSportCommandHandler(ISportRepository repository, IValidator<UpdateSportCommand> validator)
{
    public async Task<OperationResult<SportResponse>> HandleAsync(UpdateSportCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return OperationResult<SportResponse>.Validation(validationResult.ToErrorDictionary());
        }

        var sport = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (sport is null)
        {
            return OperationResult<SportResponse>.NotFound($"Sport '{command.Id}' was not found.");
        }

        sport.UpdateDetails(command.Name, command.Sectors, command.Categories);
        await repository.ReplaceAsync(sport, cancellationToken);
        return OperationResult<SportResponse>.Success(sport.ToResponse());
    }
}




