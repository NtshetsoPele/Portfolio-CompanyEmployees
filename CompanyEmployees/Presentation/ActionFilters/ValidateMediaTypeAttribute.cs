namespace Presentation.ActionFilters;

// Useful if custom media types are available - like HATEOAS.
public class ValidateMediaTypeAttribute : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (AcceptHeaderIsNotPresent())
        {
            context.Result = new BadRequestObjectResult(error: "'Accept' header is missing.");

            return;
        }

        if (AcceptHeaderCantBeParsed(out MediaTypeHeaderValue outMediaType))
        {
            context.Result = new BadRequestObjectResult(error: "Media type not present. " +
                "\nPlease add the 'Accept' header with the required media type.");

            return;
        }
        
        context.HttpContext.Items.Add("AcceptHeaderMediaType", outMediaType);

        #region Nested_Helpers

        bool AcceptHeaderIsNotPresent() =>
            !context.HttpContext.Request.Headers.ContainsKey("Accept");

        bool AcceptHeaderCantBeParsed(out MediaTypeHeaderValue mediaType) =>
            !MediaTypeHeaderValue.TryParse(input: TryGetAcceptHeader(), out mediaType);

        string? TryGetAcceptHeader() =>
            context.HttpContext.Request.Headers["Accept"].FirstOrDefault();

        #endregion
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}