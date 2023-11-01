namespace CompanyEmployees.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this WebApplication app, ILoggerManager logger)
    {
        app.UseExceptionHandler((IApplicationBuilder appBuilder) =>
        {
            appBuilder.Run(handler: async (HttpContext context) =>
            {
                context.Response.StatusCode  = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var exceptionHandlerFeature  = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionHandlerFeature != null)
                {
                    context.Response.StatusCode = exceptionHandlerFeature.Error switch
                    {
                        NotFoundException   => StatusCodes.Status404NotFound,
                        BadRequestException => StatusCodes.Status400BadRequest,
                        _ => StatusCodes.Status500InternalServerError
                    };

                    logger.LogError(message: $"Something went wrong: {exceptionHandlerFeature.Error}.");
                    await context.Response.WriteAsync(text: new ErrorDetails
                    {
                        StatusCode = context.Response.StatusCode,
                        Message    = exceptionHandlerFeature.Error.Message
                    }.ToString());
                }
            });
        });
    }
}