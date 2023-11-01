namespace Shared.DataTransferObjects.Company.Request;

// In some projects, the input and output DTO classes are the same, but it's still
// recommended to separate them for easier maintenance and refactoring of code.
// Also, output objects don't need validation, unlike input objects.
public record CompanyCreationDto(
    string Name, string Address, string Country, IEnumerable<EmployeeCreationDto>? Employees);