namespace Entities.Exceptions;

public sealed class IdParametersBadRequestException : BadRequestException
{
    public IdParametersBadRequestException() : base(message: "Parameter 'ids' is null.")
    { }
}