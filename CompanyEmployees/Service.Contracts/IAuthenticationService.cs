namespace Service.Contracts;

// Registration/Authentication logic in the service layer.
public interface IAuthenticationService
{
    Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userRegistration);
    Task<bool> ValidateUserAsync(UserAuthenticationDto userAuth);
    Task<TokenDto> CreateTokenAsync(bool populateExpiration);
    Task<TokenDto> RefreshToken(TokenDto tokenDto);
}