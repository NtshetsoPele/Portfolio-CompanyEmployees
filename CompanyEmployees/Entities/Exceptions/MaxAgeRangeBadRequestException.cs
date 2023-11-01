namespace Entities.Exceptions;

public sealed class MaxAgeRangeBadRequestException : BadRequestException
{
    public MaxAgeRangeBadRequestException() 
        : base(message: "Max age can't be less than min age.")
    { }
}