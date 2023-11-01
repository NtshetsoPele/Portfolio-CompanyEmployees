namespace Entities.Responses;

public abstract class ApiBadRequestResponse : ApiBaseResponse
{
    public string Message { get; set; }

    protected ApiBadRequestResponse(string message) : base(success: false)
    {
        Message = message;
    }
}