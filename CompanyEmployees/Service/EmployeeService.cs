namespace Service;

internal sealed class EmployeeService : IEmployeeService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly IDataShaper<EmployeeResponseDto> _dataShaper;

    public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, 
        IDataShaper<EmployeeResponseDto> dataShaper)
    {
        (_repository, _logger, _mapper, _dataShaper) = (repository, logger, mapper, dataShaper);
    }

    public async Task<EmployeeResponseDto?> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges)
    {
        _ = await _repository.Company.GetCompanyAsync(companyId, trackChanges) ?? 
            throw new CompanyNotFoundException(companyId); // Similar logic in the CompanyService.

        Employee employee = await _repository.Employee.GetEmployeeAsync(companyId, employeeId, trackChanges) ?? 
                            throw new EmployeeNotFoundException(employeeId);

        return _mapper.Map<EmployeeResponseDto>(employee);
    }

    public async Task<EmployeeResponseDto> CreateEmployeeForCompany(
        Guid companyId, EmployeeCreationDto employeeCreationDto, bool trackChanges)
    {
        _ = await _repository.Company.GetCompanyAsync(companyId, trackChanges) ??
            throw new CompanyNotFoundException(companyId);

        var employeeEntity = _mapper.Map<Employee>(employeeCreationDto);
        _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
        await _repository.SaveAsync();
        var employeeToReturn = _mapper.Map<EmployeeResponseDto>(employeeEntity);
        return employeeToReturn;
    }

    public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid employeeId, bool trackChanges)
    {
        _ = await _repository.Company.GetCompanyAsync(companyId, trackChanges) ??
            throw new CompanyNotFoundException(companyId);
        
        Employee employee = await _repository.Employee.GetEmployeeAsync(companyId, employeeId, trackChanges) ??
                            throw new EmployeeNotFoundException(employeeId);
        
        _repository.Employee.DeleteEmployee(employee);
        await _repository.SaveAsync();
    }

    public async Task<(IEnumerable<ExpandoObject> Employees, MetaData MetaData)> 
        GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
    {
        if (!employeeParameters.ValidAgeRange)
        {
            throw new MaxAgeRangeBadRequestException();
        }

        _ = await _repository.Company.GetCompanyAsync(companyId, trackChanges) ?? 
            throw new CompanyNotFoundException(companyId);

        PagedList<Employee> dBEmployeesWithMetaData = await 
            _repository.Employee.GetEmployeesAsync(companyId, employeeParameters, trackChanges);

        var employeesToReturn = _mapper.Map<IEnumerable<EmployeeResponseDto>>(dBEmployeesWithMetaData);

        IEnumerable<ExpandoObject> shapedData = 
            _dataShaper.ShapeData(employeesToReturn, employeeParameters.Fields); // Nullability handled.

        return (shapedData, dBEmployeesWithMetaData.MetaData);
    }

    // This is a connected update.
    // The same context object is used to fetch the entity and update it.
    public void UpdateEmployeeForCompany(Guid companyId, Guid employeeId, EmployeeUpdateDto employeeUpdateDto, 
        bool companyTrackChanges, bool employeeTrackChanges)
    {
        _ = _repository.Company.GetCompanyAsync(companyId, companyTrackChanges) ??
            throw new CompanyNotFoundException(companyId);

        var employee = _repository.Employee.GetEmployeeAsync(companyId, employeeId, employeeTrackChanges) ??
                       throw new EmployeeNotFoundException(employeeId);

        _mapper.Map(source: employeeUpdateDto, destination: employee);
        _repository.SaveAsync();
    }

    public async Task<(EmployeeUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(
        Guid companyId, Guid employeeId, bool companyTrackChanges, bool employeeTrackChanges)
    {
        _ = await _repository.Company.GetCompanyAsync(companyId, companyTrackChanges) ??
            throw new CompanyNotFoundException(companyId);

        Employee employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, employeeId, employeeTrackChanges) ??
                                  throw new EmployeeNotFoundException(companyId);

        var employeeToPatch = _mapper.Map<EmployeeUpdateDto>(employeeEntity);
        return (employeeToPatch, employeeEntity);
    }

    public void SaveChangesForPatch(EmployeeUpdateDto employeeToPatch, Employee employeeEntity)
    {
        _mapper.Map(source: employeeToPatch, destination: employeeEntity);
        _repository.SaveAsync();
    }
}