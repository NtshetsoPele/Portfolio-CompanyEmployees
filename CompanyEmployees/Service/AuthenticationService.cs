namespace Service;

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    // Used to provide the APIs for managing users in a persistence store.
    // Not concerned with how user information is stored.
    // Relies on a UserStore - EF Core, here.
    private readonly UserManager<User> _userManager;
    private readonly JwtConfiguration _jwtConfiguration;
    private User? _user;

    public AuthenticationService(ILoggerManager logger, IMapper mapper,
        UserManager<User> userManager, IOptions<JwtConfiguration> jwtConfig)
    {
        _logger = logger;
        _mapper = mapper;
        _userManager = userManager;
        _jwtConfiguration = jwtConfig.Value;
    }

    public async Task<IdentityResult> RegisterUserAsync(UserRegistrationDto userRegistration)
    {
        var user = _mapper.Map<User>(userRegistration);

        IdentityResult result = await _userManager.CreateAsync(user, userRegistration.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRolesAsync(user, userRegistration.Roles);
        }

        return result;
    }

    public async Task<bool> ValidateUserAsync(UserAuthenticationDto userAuth)
    {
        _user = await _userManager.FindByNameAsync(userAuth.UserName);

        bool isValidUser = _user is { } &&
                         // Verifies the user's password against the hashed password from the database.
                         await _userManager.CheckPasswordAsync(_user, userAuth.Password);

        if (!isValidUser)
        {
            _logger.LogWarn(message: $"{nameof(ValidateUserAsync)}: " +
                                     "Authentication failed. Wrong user name or password.");
        }

        return isValidUser;
    }

    public async Task<TokenDto> CreateTokenAsync(bool populateExpiration)
    {
        SigningCredentials credentials = GetSigningCredentials(); // Recreate each time?
        IList<Claim> claims = await GetClaims();
        JwtSecurityToken token = GenerateTokenOptions(credentials, claims);

        string refreshToken = GenerateRefreshToken();
        _user!.RefreshToken = refreshToken;

        if (populateExpiration)
        {
            _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7); // Config?
        }

        await _userManager.UpdateAsync(_user);

        string? accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        return new TokenDto(accessToken, refreshToken);
    }

    private static SigningCredentials GetSigningCredentials()
    {
        // Should be greater than 128 bit.
        // https://stackoverflow.com/questions/47279947/
        //   idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
        byte[] key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET") ?? 
                                            throw new KeyNotFoundException(message: "Encryption key not found."));
        var secret = new SymmetricSecurityKey(key);
        return new (key: secret, algorithm: SecurityAlgorithms.HmacSha256);
    }

    private async Task<IList<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new(type: ClaimTypes.Name, value: _user!.UserName)
        };
        IList<string>? roles = await _userManager.GetRolesAsync(_user);
        claims.AddRange(collection:
            roles.Select((string role) => new Claim(type: ClaimTypes.Role, value: role)));
        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials credentials, IEnumerable<Claim> claims)
    {
        return new JwtSecurityToken(
            issuer: _jwtConfiguration.ValidIssuer,
            audience: _jwtConfiguration.ValidAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
            signingCredentials: credentials
        );
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        // Generate random numbers that can be used in cryptographic operations.
        // Relies on the operating system to generate the numbers.
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = GenerateTokenValidationParameters();
        ClaimsPrincipal principal = GetClaimsPrincipal(out SecurityToken? securityToken);
        if (TokenIsInvalid())
        {
            throw new SecurityTokenException(message: "Invalid token");
        }
        return principal;

        #region Nested_Helpers

        TokenValidationParameters GenerateTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET") ??
                                           throw new KeyNotFoundException(message: "Encryption key not found."))),
                // Sometimes client app may want to refresh the token before it expires, enforced here.
                // If refreshing expired tokens is to also be included, this should be false.
                // Otherwise, 'TokenHandler.ValidateToken' generates an error if an expired access token is sent.
                ValidateLifetime = true,
                ValidIssuer = _jwtConfiguration.ValidIssuer,
                ValidAudience = _jwtConfiguration.ValidAudience
            }; ; 
        }

        ClaimsPrincipal GetClaimsPrincipal(out SecurityToken? securityToken)
        {
            return new JwtSecurityTokenHandler().ValidateToken(
                token, tokenValidationParameters, out securityToken);
        }

        bool TokenIsInvalid()
        {
            return securityToken is not JwtSecurityToken jwtSecurityToken ||
                   !jwtSecurityToken.Header.Alg.Equals(value: SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }

    public async Task<TokenDto> RefreshToken(TokenDto tokenDto)
    {
        ClaimsPrincipal principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
        User user = await _userManager.FindByNameAsync(principal.Identity!.Name);
        if (RefreshTokenValidationFailed())
        {
            throw new RefreshTokenBadRequest();
        }
        _user = user;
        return await CreateTokenAsync(populateExpiration: false);

        #region Nested_Helper

        bool RefreshTokenValidationFailed()
        {
            return user == null || user.RefreshToken != tokenDto.RefreshToken ||
                   // Existing refresh token has expired.
                   user.RefreshTokenExpiryTime <= DateTime.Now;
        }

        #endregion
    }
}