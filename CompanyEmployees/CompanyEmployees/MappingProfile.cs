namespace CompanyEmployees;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Company, CompanyResponseDto>()
            .ForMember(destinationMember: (CompanyResponseDto cDto) => cDto.FullAddress,
                memberOptions: opt => 
                    opt.MapFrom(mapExpression: (Company c) => string.Join(' ', c.Address, c.Country)));

        CreateMap<Employee, EmployeeResponseDto>();
        CreateMap<CompanyCreationDto, Company>();
        CreateMap<EmployeeCreationDto, Employee>();
        CreateMap<EmployeeUpdateDto, Employee>().ReverseMap();
        CreateMap<CompanyUpdateDto, Company>();
        CreateMap<UserRegistrationDto, User>();
    }
}