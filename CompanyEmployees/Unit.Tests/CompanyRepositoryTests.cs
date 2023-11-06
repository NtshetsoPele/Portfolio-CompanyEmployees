namespace Unit.Tests;

public class CompanyRepositoryTests
{
    [Fact]
    public async Task GetAllCompaniesAsync_ReturnsListOfCompanies_WithASingleCompany()
    {
        // Arrange
        var mockRepo = new Mock<ICompanyRepository>();
        mockRepo
            .Setup(expression: (ICompanyRepository repo) => repo.GetAllCompaniesAsync(false))
            .Returns(Task.FromResult(GetCompanies()));

        // Act
        IEnumerable<Company> result = await mockRepo.Object.GetAllCompaniesAsync(trackChanges: false);

        // Assert
        Assert.IsType<List<Company>>(result);
        Assert.Single(result);
    }

    private static IEnumerable<Company> GetCompanies()
    {
        return new List<Company>
        {
            new()
            {
                Id      = Guid.NewGuid(),
                Name    = "Test Company",
                Country = "United States",
                Address = "908 Woodrow Way"
            }
        };
    }
}