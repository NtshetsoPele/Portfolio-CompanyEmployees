namespace Presentation.Controllers;

// It's good practice to have a separate endpoint for the refresh token.
[Route(template: "api/token")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly IServiceManager _service;

    public TokenController(IServiceManager service) => _service = service;

    // Usually, in client applications, the 'exp' claim of the access token
    // would get inspected and if it's about to expire, the client app sends
    // the request to the api/token endpoint to get a new set of valid tokens.
    [HttpPost(template: "refresh")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Refresh([FromBody] TokenDto tokenDto)
    {
        TokenDto tokenDtoToReturn = await
            _service.AuthenticationService.RefreshToken(tokenDto);

        return Ok(tokenDtoToReturn);
    }
}