namespace Service;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<ICompanyService> _companyService;
    private readonly Lazy<IEmployeeService> _employeeService;
    private readonly Lazy<IAuthenticationService> _authenticationService;

    public ICompanyService CompanyService => _companyService.Value;
    public IEmployeeService EmployeeService => _employeeService.Value;
    public IAuthenticationService AuthenticationService => _authenticationService.Value;

    public ServiceManager(IRepositoryManager repository, ILoggerManager logger, 
        IMapper mapper, CompanyValidator validator, IDataShaper<EmployeeResponseDto> dataShaper,
        UserManager<User> userManager, IOptions<JwtConfiguration> jwtConfig)
    {
        _companyService = new Lazy<ICompanyService>(() => new
            CompanyService(repository, logger, mapper, validator));
        _employeeService = new Lazy<IEmployeeService>(() => new
            EmployeeService(repository, logger, mapper, dataShaper));
        _authenticationService = new Lazy<IAuthenticationService>(() =>
            new AuthenticationService(logger, mapper, userManager, jwtConfig));
    }
}