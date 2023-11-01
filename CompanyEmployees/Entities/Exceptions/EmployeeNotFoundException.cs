namespace Entities.Exceptions;

public sealed class EmployeeNotFoundException : NotFoundException
{
    public EmployeeNotFoundException(Guid employeeId) : 
        base(message: $"Employee with id: '{employeeId}' doesn't exist in the database.")
    { }
}