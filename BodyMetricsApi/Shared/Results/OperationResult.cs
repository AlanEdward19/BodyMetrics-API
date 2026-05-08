using Microsoft.AspNetCore.Mvc;

namespace BodyMetricsApi.Shared.Results;

public class OperationResult
{
    protected OperationResult(bool isSuccess, int statusCode, string? errorMessage = null, Dictionary<string, string[]>? errors = null)
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
        Errors = errors;
    }

    public bool IsSuccess { get; }

    public int StatusCode { get; }

    public string? ErrorMessage { get; }

    public Dictionary<string, string[]>? Errors { get; }

    public static OperationResult Success(int statusCode = StatusCodes.Status204NoContent) => new(true, statusCode);

    public static OperationResult NotFound(string message) => new(false, StatusCodes.Status404NotFound, message);

    public static OperationResult Validation(Dictionary<string, string[]> errors) => new(false, StatusCodes.Status400BadRequest, errors: errors);
}

public sealed class OperationResult<T> : OperationResult
{
    private OperationResult(bool isSuccess, int statusCode, T? value = default, string? errorMessage = null, Dictionary<string, string[]>? errors = null)
        : base(isSuccess, statusCode, errorMessage, errors)
    {
        Value = value;
    }

    public T? Value { get; }

    public static OperationResult<T> Success(T value, int statusCode = StatusCodes.Status200OK) => new(true, statusCode, value);

    public static new OperationResult<T> NotFound(string message) => new(false, StatusCodes.Status404NotFound, errorMessage: message);

    public static new OperationResult<T> Validation(Dictionary<string, string[]> errors) => new(false, StatusCodes.Status400BadRequest, errors: errors);
}

public static class OperationResultControllerExtensions
{
    public static ActionResult ToActionResult(this ControllerBase controller, OperationResult result)
    {
        if (result.IsSuccess)
        {
            return new StatusCodeResult(result.StatusCode);
        }

        if (result.Errors is not null)
        {
            return new BadRequestObjectResult(new ValidationProblemDetails(result.Errors));
        }

        return new ObjectResult(new ProblemDetails
        {
            Status = result.StatusCode,
            Title = result.StatusCode == StatusCodes.Status404NotFound ? "Resource not found" : "Request failed",
            Detail = result.ErrorMessage
        })
        {
            StatusCode = result.StatusCode
        };
    }

    public static ActionResult ToActionResult<T>(this ControllerBase controller, OperationResult<T> result)
    {
        if (result.IsSuccess)
        {
            return new ObjectResult(result.Value)
            {
                StatusCode = result.StatusCode
            };
        }

        return controller.ToActionResult((OperationResult)result);
    }
}


