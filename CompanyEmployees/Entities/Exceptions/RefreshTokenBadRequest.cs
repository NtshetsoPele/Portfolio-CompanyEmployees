namespace Entities.Exceptions;

public sealed class RefreshTokenBadRequest : BadRequestException
{
    public RefreshTokenBadRequest()
        : base(message: "Invalid client request. 'tokenDto' has some invalid values.")
    { }
}