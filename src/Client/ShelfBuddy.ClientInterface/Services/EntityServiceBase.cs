using ErrorOr;
using System.Net;
using System.Net.Http.Json;

namespace ShelfBuddy.ClientInterface.Services;

public abstract class EntityServiceBase
{
    protected virtual async Task<List<Error>> GetHttpErrorsAsync(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var validationErrorResponse = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            if (validationErrorResponse is not null)
            {
                List<Error> errors = [];
                foreach (var error in validationErrorResponse.Errors)
                {
                    errors.AddRange(error.Value.Select(errorDescription =>
                        Error.Validation(code: error.Key, description: errorDescription)));
                }

                return errors;
            }
        }

        var errorResponse = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        if (errorResponse is not null)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.NotFound => [Error.NotFound(code: errorResponse.Title ?? "NotFound",
                    description: errorResponse.Detail ?? response.ReasonPhrase ?? "Cannot find the inventory")],
                HttpStatusCode.Unauthorized => [Error.Unauthorized(code: errorResponse.Title ?? "Unauthorized",
                    description: errorResponse.Detail ?? response.ReasonPhrase ?? "You are not logged in")],
                HttpStatusCode.Forbidden => [Error.Forbidden(code: errorResponse.Title ?? "Forbidden",
                    description: errorResponse.Detail ?? response.ReasonPhrase ?? "You are not authorized for this request")],
                _ => [Error.Failure(code: errorResponse.Title ?? "UnknownFailure",
                    description: errorResponse.Detail ?? response.ReasonPhrase ?? "UnknownError")]
            };
        }

        return [Error.Failure(code: "UnknownFailure", description: response.ReasonPhrase ?? "UnknownError")];
    }
}