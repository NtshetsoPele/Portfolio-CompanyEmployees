namespace Repository.UnitOfWork;

public class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _context;
    // Repository instances are only created when accessed for the first time.
    private readonly Lazy<ICompanyRepository> _companyRepository;
    private readonly Lazy<IEmployeeRepository> _employeeRepository;

    public RepositoryManager(RepositoryContext context)
    {
        _context = context;
        _companyRepository = 
            new Lazy<ICompanyRepository>(() => new CompanyRepository(context));
        _employeeRepository = 
            new Lazy<IEmployeeRepository>(() => new EmployeeRepository(context));
    }

    public ICompanyRepository Company => _companyRepository.Value;

    public IEmployeeRepository Employee => _employeeRepository.Value;

    public async Task SaveAsync() => await _context.SaveChangesAsync();
}