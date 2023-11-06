namespace CompanyEmployees.Extensions;

public static class ServiceExtensions
{
    // CORS (Cross-Origin Resource Sharing); mechanism to give or restrict access rights
    // to applications from different domains
    public static void ConfigureCors(this IServiceCollection services) =>
        services.AddCors((CorsOptions options) =>
        {
            options.AddPolicy(name: "CorsPolicy", configurePolicy: (CorsPolicyBuilder builder) =>
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("X-Pagination"));
        });

    // Controls how the application interacts with IIS. Defaults are fine.
    public static void ConfigureIisIntegration(this IServiceCollection services) =>
        services.Configure<IISOptions>(options => { });

    public static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerManager, LoggerManager>();

    public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration config) =>
        services.AddDbContext<RepositoryContext>(optionsAction: (DbContextOptionsBuilder opts) =>
            opts.UseSqlServer(connectionString: config.GetConnectionString(name: "sqlConnection")));
            //opts.UseInMemoryDatabase(databaseName: "CompanyEmployees"));

    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();

    public static void ConfigureServiceManager(this IServiceCollection services) =>
        services.AddScoped<IServiceManager, ServiceManager>();

    public static IMvcBuilder AddCustomCsvFormatter(this IMvcBuilder builder) =>
        builder.AddMvcOptions((MvcOptions options) => 
            options.OutputFormatters.Add(new CsvOutputFormatter()));

    public static void AddCustomMediaTypes(this IServiceCollection services)
    {
        services.Configure<MvcOptions>(options =>
        {
            var systemTextJsonOutputFormatter = options.OutputFormatters
                .OfType<SystemTextJsonOutputFormatter>()
                .FirstOrDefault();
            systemTextJsonOutputFormatter?.SupportedMediaTypes
                // vnd – vendor prefix; always there
                // codemaze – vendor identifier
                // hateoas – media type name
                // json – suffix; use it to specify that json or an XML response
                .Add("application/vnd.codemaze.hateoas+json");
            var xmlOutputFormatter = options.OutputFormatters
                .OfType<XmlDataContractSerializerOutputFormatter>()?
                .FirstOrDefault();
            xmlOutputFormatter?.SupportedMediaTypes
                .Add("application/vnd.codemaze.hateoas+xml");
        });
    }

    public static void ConfigureVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning((ApiVersioningOptions options) =>
        {
            options.ReportApiVersions = true; // Added to the response header
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(majorVersion: 1, minorVersion: 0);
            options.ApiVersionReader = new HeaderApiVersionReader(headerNames: "api-version");

            /*
              Alternatively. Then remove the 'ApiVersionAttribute' decorating the controllers
              options.Conventions.Controller<CompaniesController>()
                  .HasApiVersion(new ApiVersion(1, 0));
              options.Conventions.Controller<CompaniesV2Controller>()
                  .HasDeprecatedApiVersion(new ApiVersion(2, 0));
            */
        });
    }

    public static void ConfigureResponseCaching(this IServiceCollection services) =>
        services.AddResponseCaching();

    /*
     * https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/ETag
     * The ETag (or entity tag) HTTP response header is an identifier for a specific version of a resource.
     * It lets caches be more efficient and save bandwidth, as a web server does not need to resend a full
     * response if the content was not changed. Additionally, ETags help to prevent simultaneous updates of
     * a resource from overwriting each other.
     * If the resource at a given URL changes, a new Etag value must be generated. A comparison of them can
     * determine whether two representations of a resource are the same.
    */
    /*
     * https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/If-None-Match
     * The If-None-Match HTTP request header makes the request conditional.
     * For GET and HEAD methods, the server will return the requested resource, with a 200 status, only if
     * it doesn't have an ETag matching the given ones. 
     */
    /*
     * For validation:
     * So, the same request is sent, but we don’t know if the response is valid. 
     * Therefore, the cache forwards that request to the API with the additional headers If-None-Match,
     * which is set to the Etag value and If-Modified-Since, which is set to the Last-Modified value.
     * If this request checks out against the validators, the API doesn't have to recreate the same
     * response; it just sends a 304 Not Modified status. After that, the regular response is served
     * from the cache. Of course, if this doesn't check out, a new response must be generated.
     *
     * To support validation, use the Marvin.Cache.Headers library. This library supports HTTP cache
     * headers like Cache-Control, Expires, Etag, and Last-Modified and also implements validation and
     * expiration models.
     */
    /*
     * The ResponseCaching library doesn't correctly implement the validation model. It is much better for
     * just expiration.
     *
     * Correct Etag validation:
     *  - The client should send a valid request and it is up to the Cache to add an If-None-Match tag.
     *  - Then, it is up to the server to return a 304 message to the cache and then the cache should
     *    return the same response.
     */
    // Global configuration.
    public static void ConfigureHttpCacheHeaders(this IServiceCollection services) =>
        services.AddHttpCacheHeaders(
            expirationModelOptionsAction: (ExpirationModelOptions options) =>
            {
                options.MaxAge = 65;
                // The API doesn't cache private responses.
                options.CacheLocation = CacheLocation.Public;
            },
            validationModelOptionsAction: (ValidationModelOptions options) =>
            {
                options.MustRevalidate = true;
            });

    public static void ConfigureRateLimitingOptions(this IServiceCollection services)
    {
        // 30 requests are allowed in a five-minute period for any endpoint in the API.
        var rateLimitRules = new List<RateLimitRule>
        {
            new()
            {
                Endpoint = "*",
                Limit    = 30,
                Period   = "5m"
            }
        };
        services.Configure<IpRateLimitOptions>(options =>
        {
            options.GeneralRules = rateLimitRules;
        });
        // Stores rate limit counters, policies and adding configuration.
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
    }

    // Adds and configures Identity for the User type with the given role.
    public static void ConfigureIdentity(this IServiceCollection services)
    {
        _ = services.AddIdentity<User, IdentityRole>(setupAction: (IdentityOptions options) =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 10;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();
    }

    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfiguration = new JwtConfiguration();
        configuration.Bind(JwtConfiguration.Key, jwtConfiguration);

        // cmd command: setx SECRET "SecretKey" /M
        string secretKey = Environment.GetEnvironmentVariable("SECRET") ??
                           throw new KeyNotFoundException(message: "Encryption key not found.");
        services.AddAuthentication(configureOptions: (AuthenticationOptions options) =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer((JwtBearerOptions options) =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // The issuer is the actual server that created the token.
                    ValidateIssuer = true,
                    // The receiver of the token is a valid recipient.
                    ValidateAudience = true,
                    // The token has not expired.
                    ValidateLifetime = true,
                    // The signing key is valid and is trusted by the server.
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfiguration.ValidIssuer,
                    ValidAudience = jwtConfiguration.ValidAudience,
                    IssuerSigningKey = new
                        SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    // https://stackoverflow.com/questions/61909514
                    //   /jwt-token-expiration-not-working-in-asp-net-core-api
                    // The clock skew setting defaults to 5 minutes, allowing
                    // tokens to be considered valid max 5 minutes after expiry.
                    // Clock skew setting exists because the server that issues
                    // the token and the server that validates the token might
                    // have slight differences in their clocks.
                    ClockSkew = TimeSpan.FromMinutes(value: 2) // .Zero to eliminate
                };
            });
    }

    public static void AddJwtConfiguration(
        this IServiceCollection services, IConfiguration configuration) =>
            services.Configure<JwtConfiguration>(configuration.GetSection(key: JwtConfiguration.Key));
        
    // Swashbuckle.AspNetCore.Swagger: This contains the Swagger object
    // model and the middleware to expose SwaggerDocument objects as JSON.

    // Swashbuckle.AspNetCore.SwaggerGen: A Swagger generator that builds
    // SwaggerDocument objects directly from our routes, controllers,
    // and models.

    // Swashbuckle.AspNetCore.SwaggerUI: An embedded version of the Swagger
    // UI tool. It interprets Swagger JSON to build a rich, customizable
    // experience for describing web API functionality.
    public static void ConfigureSwagger(this IServiceCollection services)
    {
        // Two versions because there's two versions of the Companies controller.
        services.AddSwaggerGen((SwaggerGenOptions options) =>
        {
            options.SwaggerDoc(name: "v1", new OpenApiInfo
            {
                Title          = "My API",
                Version        = "v1",
                Description    = "CompanyEmployees API by Lesabasaba Pty Ltd",
                TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name  = "John Doe",
                    Email = "John.Doe@gmail.com",
                    Url   = new Uri("https://twitter.com/johndoe"),
                },
                License = new OpenApiLicense
                {
                    Name = "CompanyEmployees API LICX",
                    Url  = new Uri("https://example.com/license"),
                }
            });
            options.SwaggerDoc(name: "v2", new OpenApiInfo
            {
                Title   = "My API",
                Version = "v2"
            });

            var xmlFile = $"{typeof(Presentation.AssemblyReference).Assembly.GetName().Name}.xml";
            string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);

            options.AddSecurityDefinition(name: "Bearer", new OpenApiSecurityScheme
            {
                In          = ParameterLocation.Header,
                Description = "Place to add JWT with Bearer",
                Name        = "Authorization",
                Type        = SecuritySchemeType.ApiKey,
                Scheme      = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        },
                        Name = "Bearer",
                    },
                    new List<string>()
                }
            });
        });
    }
}