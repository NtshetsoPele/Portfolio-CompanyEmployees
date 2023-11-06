namespace Unit.Tests;

public class CompaniesControllerTests
{
    [Fact]
    public async Task GetAllCompaniesAsync_ReturnsListOfAllCompanies()
    {
        // Arrange
        var mockServiceManager = new Mock<IServiceManager>();
        var mockCompanyService = new Mock<ICompanyService>();
        var companies = new List<CompanyResponseDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Company 1" },
            new() { Id = Guid.NewGuid(), Name = "Company 2" }
        };
        mockCompanyService
            .Setup(expression: (ICompanyService s) => s.GetAllCompaniesAsync(false))
            .ReturnsAsync(new ApiOkResponse<IEnumerable<CompanyResponseDto>>(companies));
        mockServiceManager
            .Setup(expression: (IServiceManager s) => s.CompanyService)
            .Returns(mockCompanyService.Object);
        var controller = new CompaniesController(mockServiceManager.Object);

        // Act
        IActionResult result = await controller.GetCompanies();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCompanies = Assert.IsAssignableFrom<IEnumerable<CompanyResponseDto>>(okResult.Value);
        Assert.Equal(expected: companies.Count, actual: returnedCompanies.Count());
    }

    [Theory]
    [AutoData]
    public async Task GetCompany_ReturnsSingleCompany(CompanyResponseDto company)
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var mockServiceManager = new Mock<IServiceManager>();
        var mockCompanyService = new Mock<ICompanyService>();
        mockCompanyService
            .Setup(expression: (ICompanyService s) => s.GetCompanyAsync(companyId, false))
            .ReturnsAsync(new ApiOkResponse<CompanyResponseDto>(result: company));
        mockServiceManager
            .Setup(expression: (IServiceManager s) => s.CompanyService)
            .Returns(mockCompanyService.Object);
        var controller = new CompaniesController(mockServiceManager.Object);

        // Act
        IActionResult result = await controller.GetCompany(companyId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCompany = Assert.IsAssignableFrom<CompanyResponseDto>(okResult.Value);
        Assert.Equal(expected: company.Id, actual: returnedCompany.Id);
        Assert.Equal(expected: company.Name, actual: returnedCompany.Name);
    }

    [Theory]
    [AutoData]
    public async Task CreateCompany_CreatesNewCompanyAndReturns201StatusCode(
        CompanyCreationDto companyDto, CompanyResponseDto newCompany)
    {
        // Arrange
        var mockServiceManager = new Mock<IServiceManager>();
        var mockCompanyService = new Mock<ICompanyService>();
        mockCompanyService
            .Setup(expression: (ICompanyService s) => s.CreateCompanyAsync(companyDto))
            .ReturnsAsync(newCompany);
        mockServiceManager
            .Setup(expression: (IServiceManager s) => s.CompanyService)
            .Returns(mockCompanyService.Object);
        var controller = new CompaniesController(mockServiceManager.Object);

        // Act
        IActionResult result = await controller.CreateCompany(companyDto);

        // Assert
        var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result);
        Assert.Equal(expected: "CompanyById", actual: createdAtRouteResult.RouteName);
        Assert.Equal(expected: newCompany.Id, actual: createdAtRouteResult.RouteValues!["companyId"]);
        var returnedCompany = Assert.IsAssignableFrom<CompanyResponseDto>(createdAtRouteResult.Value);
        Assert.Equal(expected: newCompany.Id, actual: returnedCompany.Id);
        Assert.Equal(expected: newCompany.Name, actual: returnedCompany.Name);
    }

    [Fact]
    public async Task GetCompany_Returns404StatusCode()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var mockServiceManager = new Mock<IServiceManager>();
        var mockCompanyService = new Mock<ICompanyService>();
        mockCompanyService
            .Setup(expression: s => s.GetCompanyAsync(companyId, It.IsAny<bool>()))
            .ReturnsAsync(new CompanyNotFoundResponse(id: companyId));
        mockServiceManager
            .Setup(expression: (IServiceManager s) => s.CompanyService)
            .Returns(mockCompanyService.Object);
        var controller = new CompaniesController(mockServiceManager.Object);

        // Act
        var result = await controller.GetCompany(companyId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var errorDetails = Assert.IsAssignableFrom<ErrorDetails>(notFoundResult.Value);
        Assert.Equal(expected: StatusCodes.Status404NotFound, actual: errorDetails.StatusCode);
    }
}