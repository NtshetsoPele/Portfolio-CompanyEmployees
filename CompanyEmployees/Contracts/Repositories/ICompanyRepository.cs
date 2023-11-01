namespace Contracts.Repositories;

public interface ICompanyRepository
{
    Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges);
    Task<Company?> GetCompanyAsync(Guid companyId, bool trackChanges);
    Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);

    // Left synchronous. No changes made to the db.
    // The state of the entity gets marked for addition/deletion.
    void CreateCompany(Company company);
    void DeleteCompany(Company company);
}