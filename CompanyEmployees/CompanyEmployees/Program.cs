WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

LoggingUtilities.SetupLogging();

builder.Services.ConfigureCors();
builder.Services.ConfigureIisIntegration();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<CompanyValidator>();
builder.Services.AddScoped<IDataShaper<EmployeeResponseDto>, DataShaper<EmployeeResponseDto>>();
// Suppressing the default model state validation that is implemented due to the existence
// of the [ApiController] attribute.
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddScoped<ValidationFilterAttribute>();
builder.Services.AddScoped<ValidateMediaTypeAttribute>();
// Registers only the controllers in IServiceCollection and not Views or Pages.
builder.Services.AddControllers(configure: (MvcOptions config) =>
    {
        config.RespectBrowserAcceptHeader = true;
        // If the client tries to negotiate for a media type that a server doesn't
        // support, it should return HTTP status code 406 'Not Acceptable'.
        config.ReturnHttpNotAcceptable = true;
        config.InputFormatters.Insert(index: 0, item: GetJsonPatchInputFormatter());
        config.CacheProfiles.Add(key: "120SecondsDuration", value: new CacheProfile
        {
            Duration = 120
        });
    })
    .AddXmlDataContractSerializerFormatters()
    .AddCustomCsvFormatter()
    // Find and configure controllers with the framework. 
    .AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);
builder.Services.AddCustomMediaTypes();
builder.Services.ConfigureVersioning();
builder.Services.ConfigureResponseCaching();
builder.Services.ConfigureHttpCacheHeaders();
builder.Services.AddMemoryCache(); // Rate limiting uses a memory cache to store its counters and rules.
builder.Services.ConfigureRateLimitingOptions();
builder.Services.AddHttpContextAccessor(); // Rate limiting access.
builder.Services.ConfigureIdentity();
// builder.Services.AddAuthentication(); Next registration handles this.
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.ConfigureSwagger();

// 'WebApplication' implements multiple interfaces:
//   'IHost' -> start and stop the host,
//   'IApplicationBuilder' -> build the middleware pipeline 
//   'IEndpointRouteBuilder' -> add endpoints in the app.
WebApplication app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);
if (app.Environment.IsProduction())
{
    app.AllowHttpsOnly();
}
else
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});
app.UseIpRateLimiting();
app.UseCors("CorsPolicy");
// Microsoft recommends having 'UseCors' just before 'UseResponseCaching'.
app.UseResponseCaching();
app.UseHttpCacheHeaders();
app.UseAuthentication();
app.UseAuthorization();
// Adds the endpoints from controller actions to the IEndpointRouteBuilder.
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI((SwaggerUIOptions options) =>
{
    options.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "My API v1");
    options.SwaggerEndpoint(url: "/swagger/v2/swagger.json", name: "My API v2");
});

app.MigrateDatabase().Run();

// Configures support for JSON Patch using Newtonsoft.Json while leaving
// the other formatters unchanged.
// TODO: duplicate collection of singleton services warning?
static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
    new ServiceCollection().AddLogging().AddMvc().AddNewtonsoftJson()
        .Services.BuildServiceProvider()
        .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
        .OfType<NewtonsoftJsonPatchInputFormatter>().First();

/*
static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter(IServiceProvider serviceProvider)
{
    var mvcOptions = serviceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;
    return mvcOptions.InputFormatters.OfType<NewtonsoftJsonPatchInputFormatter>().First();
}
*/

public partial class Program { }
