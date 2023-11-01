namespace Entities.Exceptions;

public sealed class CompanyNotFoundException : NotFoundException
{
    public CompanyNotFoundException(Guid companyId) : 
        base(message: $"The company with id: '{companyId}' doesn't exist in the database.")
    { }
}