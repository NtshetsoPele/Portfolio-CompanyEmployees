namespace Presentation.Controllers;

public class ApiControllerBase : ControllerBase
{
    protected IActionResult ProcessError(ApiBaseResponse baseResponse)
    {
        return baseResponse switch
        {
            ApiNotFoundResponse response => NotFound(new ErrorDetails
            {
                Message    = response.Message,
                StatusCode = StatusCodes.Status404NotFound
            }),
            ApiBadRequestResponse response => BadRequest(new ErrorDetails
            {
                Message    = response.Message,
                StatusCode = StatusCodes.Status400BadRequest
            }),
            _ => throw new NotImplementedException() // For now.
        };
    }
}