namespace Shared.DataTransferObjects.Company.Request;

public record CompanyUpdateDto(
    string Name, string Address, string Country, IEnumerable<EmployeeCreationDto>? Employees);