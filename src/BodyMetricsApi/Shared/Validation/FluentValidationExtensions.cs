using FluentValidation.Results;

namespace BodyMetricsApi.Shared.Validation;

public static class FluentValidationExtensions
{
    public static Dictionary<string, string[]> ToErrorDictionary(this ValidationResult validationResult)
    {
        return validationResult.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).Distinct().ToArray());
    }
}

