namespace Presentation.Controllers;

[ApiController]
[Route(template: "api/companies/{companyId:guid}/employees")]
public class EmployeesController : ControllerBase
{
    private readonly IServiceManager _service;

    public EmployeesController(IServiceManager service) => _service = service;

    [HttpGet(template: "{employeeId:guid}", Name = "GetEmployeeForCompany")]
    public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid employeeId)
    {
        EmployeeResponseDto? employee = await 
            _service.EmployeeService.GetEmployeeAsync(companyId, employeeId, trackChanges: false);

        return Ok(employee);
    }

    [HttpGet]
    [HttpHead]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    public async Task<IActionResult> GetEmployeesForCompany(
        Guid companyId, [FromQuery] EmployeeParameters employeeParameters)
    {
        (IEnumerable<ExpandoObject> employees, MetaData metaData)  = await
            _service.EmployeeService.GetEmployeesAsync(companyId, employeeParameters, trackChanges: false);

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metaData));

        return Ok(employees);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployeeForCompany(
        Guid companyId, [FromBody] EmployeeCreationDto? employeeCreationDto)
    {
        if (employeeCreationDto is null)
        {
            return BadRequest(error: "No new employee was supplied.");
        }

        if (!ModelState.IsValid)
        {
            // Add another error message key-value pair.
            ModelState.AddModelError(key: "key", errorMessage: "errorMessage"); 

            return UnprocessableEntity(ModelState);
        }

        EmployeeResponseDto employeeToReturn = await 
            _service.EmployeeService.CreateEmployeeForCompany(companyId, employeeCreationDto, trackChanges: false);

        return CreatedAtRoute(routeName: "GetEmployeeForCompany", 
            routeValues: new
            {
                companyId, employeeId = employeeToReturn.Id
            },
            value: employeeToReturn);
    }

    [HttpDelete(template: "{employeeId:guid}")]
    public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid employeeId)
    {
        await _service.EmployeeService.DeleteEmployeeForCompanyAsync(companyId, employeeId, trackChanges: false);

        return NoContent();
    }

    // PUT is a request for a full update
    [HttpPut(template: "{employeeId:guid}")]
    public IActionResult UpdateEmployeeForCompany(
        Guid companyId, Guid employeeId, [FromBody] EmployeeUpdateDto? employeeUpdateDto)
    {
        if (employeeUpdateDto is null)
        {
            return BadRequest(error: "No update details sent for an employee.");
        }

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        _service.EmployeeService.UpdateEmployeeForCompany(companyId, employeeId, employeeUpdateDto,
            companyTrackChanges: false, employeeTrackChanges: true);

        return NoContent();
    }

    // PATCH is a request for a partial update.
    [HttpPatch(template: "{employeeId:guid}")]
    public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(
        Guid companyId, Guid employeeId, [FromBody] JsonPatchDocument<EmployeeUpdateDto>? patchDoc)
    {
        if (patchDoc is null)
        {
            return BadRequest(error: "patchDoc object sent from client is null.");
        }

        var (employeeToPatch, employeeEntity) = await _service.EmployeeService.GetEmployeeForPatchAsync(
            companyId, employeeId, companyTrackChanges: false, employeeTrackChanges: true);

        // 'pathDoc' can only apply to EmployeeUpdateDto.
        // 'pathDoc' comes from the 'JsonPatch' namespace.
        // Use the 'NewtonsoftJson' namespace for 'ApplyTo'.
        patchDoc.ApplyTo(employeeToPatch, ModelState);

        // Validate the already patched 'employeeToPatch'
        TryValidateModel(employeeToPatch);

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        _service.EmployeeService.SaveChangesForPatch(employeeToPatch, employeeEntity);

        return NoContent();
    }
}