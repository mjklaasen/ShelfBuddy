using ErrorOr;

namespace ShelfBuddy.Contracts;

public record ErrorResponse(List<Error> Errors);