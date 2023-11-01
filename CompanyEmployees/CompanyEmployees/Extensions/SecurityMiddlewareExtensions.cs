namespace CompanyEmployees.Extensions;

public static class SecurityMiddlewareExtensions
{
    public static void AllowHttpsOnly(this WebApplication app)
    {
        app.Use(async (HttpContext context, RequestDelegate next) =>
        {
            if (!context.Request.IsHttps)
            {
                await context.Response.WriteAsync(text: new ErrorDetails
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message    = "HTTPS required."
                }.ToString());
            }
            else
            {
                await next(context);
            }
        });
    }
}