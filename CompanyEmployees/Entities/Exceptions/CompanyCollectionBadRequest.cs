namespace Entities.Exceptions;

public sealed class CompanyCollectionBadRequest : BadRequestException
{
    public CompanyCollectionBadRequest()
        : base(message: "Company collection sent from a client is null.")
    { }
}