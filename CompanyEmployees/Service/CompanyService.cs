namespace Service;

internal sealed class CompanyService : ICompanyService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger; // Likely used later.
    private readonly IMapper _mapper;
    private readonly CompanyValidator _validator;

    public CompanyService(
        IRepositoryManager repository, ILoggerManager logger, IMapper mapper, CompanyValidator validator)
    {
        (_repository, _logger, _mapper, _validator) = (repository, logger, mapper, validator);
    }

    public async Task<CompanyResponseDto> CreateCompanyAsync(CompanyCreationDto companyDto)
    {
        var companyEntity = _mapper.Map<Company>(companyDto);
        _repository.Company.CreateCompany(companyEntity);
        await _repository.SaveAsync();
        var companyToReturn = _mapper.Map<CompanyResponseDto>(companyEntity);
        return companyToReturn;
    }

    public async Task<ApiBaseResponse> GetAllCompaniesAsync(bool trackChanges)
    {
        IEnumerable<Company> companies = await _repository.Company.GetAllCompaniesAsync(trackChanges);
        var companyDtos = _mapper.Map<IEnumerable<CompanyResponseDto>>(companies);
        return new ApiOkResponse<IEnumerable<CompanyResponseDto>>(companyDtos);
    }

    public async Task<IEnumerable<CompanyResponseDto>> GetByIdsAsync(IList<Guid> ids, bool trackChanges)
    {
        if (ids is null)
        {
            throw new IdParametersBadRequestException();
        }

        IEnumerable<Company> companyEntities = await _repository.Company.GetByIdsAsync(ids, trackChanges);
        if (ids.Count != companyEntities.Count())
        {
            throw new CollectionByIdsBadRequestException();
        }

        var companiesToReturn = _mapper.Map<IEnumerable<CompanyResponseDto>>(companyEntities);
        return companiesToReturn;
    }

    public async Task<ApiBaseResponse> GetCompanyAsync(Guid companyId, bool trackChanges)
    {
        Company company = await TryGetCompany(companyId, trackChanges);

        var companyDto = _mapper.Map<CompanyResponseDto>(company);

        return new ApiOkResponse<CompanyResponseDto>(companyDto);
    }

    public async Task<(IEnumerable<CompanyResponseDto> companies, string ids)> CreateCompanyCollectionAsync
        (IEnumerable<CompanyCreationDto> companyCollection)
    {
        if (companyCollection is null)
        {
            throw new CompanyCollectionBadRequest();
        }
        var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
        foreach (var company in companyEntities)
        {
            _repository.Company.CreateCompany(company);
        }
        await _repository.SaveAsync();
        var companyCollectionToReturn = _mapper.Map<IList<CompanyResponseDto>>(companyEntities);
        string ids = string.Join(",", companyCollectionToReturn.Select((CompanyResponseDto c) => c.Id));
        return (companyCollectionToReturn, ids);
    }

    public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges)
    {
        Company companyEntity = await TryGetCompany(companyId, trackChanges);
        _repository.Company.DeleteCompany(companyEntity);
        await _repository.SaveAsync();
    }

    public async Task UpdateCompanyAsync(Guid companyId, CompanyUpdateDto companyForUpdate, bool trackChanges)
    {
        Company companyEntity = await _validator.TryGetCompany(companyId, trackChanges);
        _mapper.Map(source: companyForUpdate, destination: companyEntity);
        await _repository.SaveAsync();
    }

    // Pushed out to a service - alternative approach.
    private async Task<Company> TryGetCompany(Guid companyId, bool trackChanges)
    {
        return await _repository.Company.GetCompanyAsync(companyId, trackChanges) ??
                      throw new CompanyNotFoundException(companyId);
    }
}