namespace Entities.Responses;

public abstract class ApiNotFoundResponse : ApiBaseResponse
{
    public string Message { get; set; }

    protected ApiNotFoundResponse(string message) : base(success: false)
    {
        Message = message;
    }
}