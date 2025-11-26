using ErrorOr;

namespace ShelfBuddy.Contracts;

public record ErrorResponse(IList<Error> Errors);
