namespace Repository;

public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(RepositoryContext context)
        : base(context)
    { }

    public void CreateEmployeeForCompany(Guid companyId, Employee employee)
    {
        employee.CompanyId = companyId;
        Create(entity: employee);
    }

    public void DeleteEmployee(Employee employee) => Delete(employee);

    public async Task<Employee?> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges)
    {
        return await FindByCondition(expression: (Employee e) => 
                    e.CompanyId.Equals(companyId) && e.Id.Equals(employeeId), trackChanges)
            .SingleOrDefaultAsync();
    }

    public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, 
        EmployeeParameters employeeParameters, bool trackChanges)
    {
        IList<Employee> employees = await GenerateBaseQuery()
            .Sort(employeeParameters.OrderBy!)
            .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
            .Take(employeeParameters.PageSize)
            .ToListAsync();

        int countOfMatchingEmployees = await GenerateBaseQuery().CountAsync();

        return GenerateResult();

        #region Nested_Helpers

        IQueryable<Employee> GenerateBaseQuery()
        {
            return FindByCondition(expression: (Employee e) => e.CompanyId.Equals(companyId), trackChanges)
                .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
                .Search(employeeParameters.SearchTerm); // Nullability catered for.
        }

        PagedList<Employee> GenerateResult()
        {
            return PagedList<Employee>.ToPagedList(employees, countOfMatchingEmployees,
                employeeParameters.PageNumber, employeeParameters.PageSize);
        }

        #endregion
    }
}