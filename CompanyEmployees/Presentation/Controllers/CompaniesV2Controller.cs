namespace Presentation.Controllers;

[ApiController]
// If deprecated, 'api-deprecated-versions' is added to the response headers.
[ApiVersion(version: "2.0", Deprecated = true)]
[Route(template: "api/companies")]
// All the other controllers will be included in both groups because they are not versioned.
[ApiExplorerSettings(GroupName = "v2")]
//[ResponseCache(CacheProfileName = "120SecondsDuration")]
// Created just to play with some features.
public class CompaniesV2Controller : ApiControllerBase
{
    private readonly IServiceManager _service;

    public CompaniesV2Controller(IServiceManager service) => _service = service;

    /// <summary>
    ///   Gets the list of all companies
    /// </summary>
    /// <returns>The companies list</returns>
    /// <response code="201">Returns the newly created item.</response>
    /// <response code="400">If the item is null.</response>
    /// <response code="422">If the model is invalid.</response>
    [HttpGet]
    // 'Marvin.Cache.Header' overrides the below. Global settings used.
    [ResponseCache(Duration = 60)] // Overrides controller attribute. 
    // If no role is specified, any authenticated user is granted access.
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(statusCode: 201)]
    [ProducesResponseType(statusCode: 400)]
    [ProducesResponseType(statusCode: 422)]
    public async Task<IActionResult> GetCompanies()
    {
        // IHeaderDictionary headers = this.HttpContext.Request.Headers;
        ApiBaseResponse companies = await 
            _service.CompanyService.GetAllCompaniesAsync(trackChanges: false);

        // Projecting
        IEnumerable<string> companiesV2 = companies.GetResult<IEnumerable<CompanyResponseDto>>()
            .Select((CompanyResponseDto c) => $"{c.Name} 'V2'");

        return Ok(companiesV2);
    }

    [HttpGet(template: "{companyId:guid}")]
    // Resource level configuration. Replaces [ResponseCache].
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)] 
    //[HttpCacheValidation(MustRevalidate = false)]
    public async Task<IActionResult> GetCompany(Guid companyId)
    {
        ApiBaseResponse company = await 
            _service.CompanyService.GetCompanyAsync(companyId, trackChanges: false);

        if (!company.Success)
        {
            return ProcessError(company);
        }

        dynamic expando = new ExpandoObject();
        expando.Comany = company.GetResult<CompanyResponseDto>();
        expando.Version = "V2";

        return Ok(expando);
    }
}