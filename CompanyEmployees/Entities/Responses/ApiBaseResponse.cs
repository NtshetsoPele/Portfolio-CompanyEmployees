namespace Entities.Responses;

// Custom exceptions is more natural to implement.
// However, if throughput is an issue, the below approach can be considered.
public abstract class ApiBaseResponse
{
    public bool Success { get; set; }

    protected ApiBaseResponse(bool success) => Success = success;
}