namespace Presentation.ActionFilters;

// Comes from the client. It could happen that it can’t be deserialized.
// Switched off default invalid model state filter for the controllers.
public class ValidationFilterAttribute : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var (action, controller, param) = ExtractEndpointTargets();

        if (param is null)
        {
            context.Result = GetBadRequestResponse();

            return;
        }

        ReturnIfModelStateIsInvalid();

        #region Nested_Helpers

        (object?, object?, object?) ExtractEndpointTargets() => (GetAction(), GetController(), GetDto());

        object? GetAction() => context.RouteData.Values["action"];

        object? GetController() => context.RouteData.Values["controller"];

        object? GetDto()
        {
            return context.ActionArguments
                .SingleOrDefault((KeyValuePair<string, object?> argumentPair) =>
                    //argumentPair.Value!.ToString()!.Contains("Dto")).Value;
                    argumentPair.Key!.ToString()!.Contains("Dto")).Value;
        }

        BadRequestObjectResult GetBadRequestResponse()
        {
            return new BadRequestObjectResult(error:
                $"Object is null. Controller: '{controller}', action: '{action}'."); ;
        }

        void ReturnIfModelStateIsInvalid()
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
            }
        }

        #endregion
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}