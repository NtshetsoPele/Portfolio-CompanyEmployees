namespace Service.Validation;

public class CompanyValidator
{
    private readonly IRepositoryManager _repository;

    public CompanyValidator(IRepositoryManager repository) => _repository = repository;

    public async Task<Company> TryGetCompany(Guid companyId, bool trackChanges)
    {
        return await _repository.Company.GetCompanyAsync(companyId, trackChanges) ??
               throw new CompanyNotFoundException(companyId);
    }
}