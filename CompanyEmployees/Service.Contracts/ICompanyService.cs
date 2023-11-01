namespace Service.Contracts;

public interface ICompanyService
{
    Task<ApiBaseResponse> GetAllCompaniesAsync(bool trackChanges);
    Task<ApiBaseResponse> GetCompanyAsync(Guid companyId, bool trackChanges);
    Task<CompanyResponseDto> CreateCompanyAsync(CompanyCreationDto companyDto);
    Task<IEnumerable<CompanyResponseDto>> GetByIdsAsync(IList<Guid> ids, bool trackChanges);
    Task<(IEnumerable<CompanyResponseDto> companies, string ids)> CreateCompanyCollectionAsync(
        IEnumerable<CompanyCreationDto> companyCollection);
    Task DeleteCompanyAsync(Guid companyId, bool trackChanges);
    Task UpdateCompanyAsync(Guid companyId, CompanyUpdateDto companyForUpdate, bool trackChanges);
}