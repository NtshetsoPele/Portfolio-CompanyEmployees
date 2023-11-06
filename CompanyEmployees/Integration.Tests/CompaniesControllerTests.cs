using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Integration.Tests;

public class CompaniesControllerTests : IClassFixture<CustomWebAppFactory>
{
    private readonly HttpClient _client;

    public CompaniesControllerTests(CustomWebAppFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task GetCompaniesOptions_AuthenticationFirst_TwoCompaniesReturned()
    {
        // Arrange
        UserRegistrationDto newUser = GetUserRegistrationDto();
        HttpContent content = GetHttpContent(newUser);
        HttpResponseMessage response = await _client.PostAsync("api/authentication", content);

        if (response.IsSuccessStatusCode)
        {
            UserAuthenticationDto userLogins = GetAuthenticationDto();
            content = GetHttpContent(userLogins); 
            response = await _client.PostAsync("api/authentication/login", content);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                TokenDto? token = JsonConvert.DeserializeObject<TokenDto>(responseBody);
                AddAuthorization(token!.AccessToken);

                // Act
                response = await _client.GetAsync("api/companies");

                // Assert
                responseBody = await response.Content.ReadAsStringAsync();
                var companies = JsonConvert.DeserializeObject<IEnumerable<CompanyResponseDto>>(responseBody);
                Assert.Equal(expected: 2, actual: companies!.Count());

                return;
            }
        }

        Assert.False(condition: false);
    }

    private static UserRegistrationDto GetUserRegistrationDto() =>
        new()
        {
            FirstName = "John",
            LastName = "Doe",
            UserName = "JDoe",
            Password = "Password1000",
            Email = "johndoe@mail.com",
            PhoneNumber = "087-589-654",
            Roles = new[] { "Manager" }
        };

    private static HttpContent GetHttpContent(object content) =>
        new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, mediaType: "application/json");

    private static UserAuthenticationDto GetAuthenticationDto() =>
        new()
        {
            UserName = "JDoe",
            Password = "Password1000"
        };

    private void AddAuthorization(string accessToken)
    {
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(scheme: "Bearer", parameter: accessToken);
    }
}