namespace Repository;

public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
    public CompanyRepository(RepositoryContext context) : base(context)
    { }

    public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges)
    {
        return await FindAll(trackChanges).OrderBy((Company c) => c.Name).ToListAsync();
    }

    public async Task<Company?> GetCompanyAsync(Guid companyId, bool trackChanges)
    {
        return await FindByCondition(expression: (Company c) => c.Id.Equals(companyId), trackChanges)
            .SingleOrDefaultAsync();
    }

    public void CreateCompany(Company company) => Create(company);

    public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
    {
        return await FindByCondition(expression: (Company c) => ids.Contains(c.Id), trackChanges)
            .ToListAsync();
    }

    public void DeleteCompany(Company company) => Delete(company);
}