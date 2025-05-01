using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace ShelfBuddy.API.Common;

public static class CustomResults
{
    public static IResult Problem(List<Error> errors)
    {
        if (errors.Count is 0)
        {
            return Results.Problem();
        }

        if (errors.All(error => error.Type == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        return Problem(errors[0]);
    }

    private static IResult Problem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError,
        };

        return Results.Problem(statusCode: statusCode, title: error.Description);
    }

    private static IResult ValidationProblem(List<Error> errors)
    {
        var modelStateDictionary = errors.GroupBy(error => error.Code).ToDictionary(grouping => grouping.Key,
            grouping => grouping.Select(error => error.Description).ToArray());

        return Results.ValidationProblem(modelStateDictionary);
    }
}