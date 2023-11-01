namespace Entities.Exceptions;

public sealed class CollectionByIdsBadRequestException : BadRequestException
{
    public CollectionByIdsBadRequestException()
        : base(message: "Collection count mismatch compared to ids given.")
    { }
}