using Presentation.ModelBinders;

namespace Presentation.Controllers;

// Attribute routing requirement.
// Automatic HTTP 400 responses (model validation).
// Binding source parameter inference.
// Multipart/form-data request inference.
// Problem details for error status codes.
[ApiController]
[ApiVersion(version: "1.0", Deprecated = false)]
[Route(template: "api/companies")]
[ApiExplorerSettings(GroupName = "v1")]
public class CompaniesController : ApiControllerBase
{
    private readonly IServiceManager _service;

    public CompaniesController(IServiceManager service) => _service = service;

    [HttpOptions]
    public IActionResult GetCompaniesOptions()
    {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, DELETE");

        return Ok();
    }

    // Actions aren't targeted by their names but by their routes. 
    // Therefore, adding the 'Async' suffix to their names doesn't really add value?
    [HttpGet]
    [Authorize] // No role.
    public async Task<IActionResult> GetCompanies()
    {
        ApiBaseResponse companies = await
            _service.CompanyService.GetAllCompaniesAsync(trackChanges: false);

        return Ok(companies.GetResult<IEnumerable<CompanyResponseDto>>());
    }

    // Route constraint applied to the the 1st parameter
    [HttpGet(template: "{companyId:guid}", Name = "CompanyById")] 
    public async Task<IActionResult> GetCompany(Guid companyId)
    {
        ApiBaseResponse company = await 
            _service.CompanyService.GetCompanyAsync(companyId, trackChanges: false);

        return !company.Success ? 
            ProcessError(company) : Ok(company.GetResult<CompanyResponseDto>());
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyCreationDto companyDto)
    {
        // HTTP 201 - Created.
        // Populate the response body with the new resource.
        // Add a location header pointing to the new resource.
        CompanyResponseDto newCompany = await _service.CompanyService.CreateCompanyAsync(companyDto);

        return CreatedAtRoute(routeName: "CompanyById", 
            routeValues: new { companyId = newCompany.Id }, value: newCompany);
    }

    [HttpGet(template: "collection/({ids})", Name = "CompanyCollection")]
    public async Task<IActionResult> GetCompanyCollection(
        [ModelBinder(BinderType = typeof(ArrayModelBinder))] IList<Guid> ids)
    {
        IEnumerable<CompanyResponseDto> companies = 
            await _service.CompanyService.GetByIdsAsync(ids, trackChanges: false);

        return Ok(companies);
    }

    [HttpPost(template: "collection")]
    [Authorize]
    // Validate individual dts?
    public async Task<IActionResult> CreateCompanyCollection(
        [FromBody] IEnumerable<CompanyCreationDto> companyCollection)
    {
        (IEnumerable<CompanyResponseDto> returningCompanies, string returningCompanyIds) = 
            await _service.CompanyService.CreateCompanyCollectionAsync(companyCollection);

        return CreatedAtRoute(routeName: "CompanyCollection", 
            routeValues: new { ids = returningCompanyIds }, value: returningCompanies);
    }

    [HttpDelete(template: "{companyId:guid}")]
    public async Task<IActionResult> DeleteCompany(Guid companyId)
    {
        await _service.CompanyService.DeleteCompanyAsync(companyId, trackChanges: false);

        return NoContent();
    }

    [HttpPut(template: "{companyId:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [Authorize]
    public async Task<IActionResult> UpdateCompany(
        Guid companyId, [FromBody] CompanyUpdateDto companyUpdateDto)
    {
        await _service.CompanyService.UpdateCompanyAsync(
            companyId, companyUpdateDto, trackChanges: true);

        return NoContent();
    }
}