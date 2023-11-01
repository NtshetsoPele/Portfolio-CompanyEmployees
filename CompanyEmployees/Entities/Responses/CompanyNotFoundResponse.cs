namespace Entities.Responses;

public sealed class CompanyNotFoundResponse : ApiNotFoundResponse
{
    public CompanyNotFoundResponse(Guid id)
        : base(message: $"Company with id '{id}' was not found in db.")
    { }
}