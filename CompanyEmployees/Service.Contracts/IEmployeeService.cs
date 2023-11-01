namespace Service.Contracts;

public interface IEmployeeService
{
    Task<(IEnumerable<ExpandoObject> Employees, MetaData MetaData)> GetEmployeesAsync(Guid companyId, 
        EmployeeParameters employeeParameters, bool trackChanges);
    Task<EmployeeResponseDto?> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges);
    Task<EmployeeResponseDto> CreateEmployeeForCompany(
        Guid companyId, EmployeeCreationDto employeeCreationDto, bool trackChanges);
    Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid employeeId, bool trackChanges);
    void UpdateEmployeeForCompany(Guid companyId, Guid employeeId,
        EmployeeUpdateDto employeeUpdateDto, bool companyTrackChanges, bool employeeTrackChanges);
    Task<(EmployeeUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(
        Guid companyId, Guid employeeId, bool companyTrackChanges, bool employeeTrackChanges);
    void SaveChangesForPatch(EmployeeUpdateDto employeeToPatch, Employee employeeEntity);
}