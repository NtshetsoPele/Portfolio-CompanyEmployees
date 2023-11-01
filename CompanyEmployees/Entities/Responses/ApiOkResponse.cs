namespace Entities.Responses;

public sealed class ApiOkResponse<TResult> : ApiBaseResponse
{
    public TResult Result { get; set; }

    public ApiOkResponse(TResult result) : base(success: true)
    {
        Result = result;
    }
}