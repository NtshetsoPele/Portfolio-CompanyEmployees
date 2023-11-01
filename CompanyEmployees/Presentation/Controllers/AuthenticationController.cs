namespace Presentation.Controllers;

[ApiController]
[Route(template: "api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IServiceManager _service;

    public AuthenticationController(IServiceManager service) => _service = service;

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto userRegistration)
    {
        IdentityResult result = await _service.AuthenticationService.RegisterUserAsync(userRegistration);

        if (result.Succeeded)
        {
            return StatusCode(201);
        }

        foreach (IdentityError error in result.Errors)
        {
            ModelState.TryAddModelError(key: error.Code, errorMessage: error.Description);
        }

        return BadRequest(ModelState);
    }

    [HttpPost(template: "login")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Authenticate([FromBody] UserAuthenticationDto user)
    {
        if (!await _service.AuthenticationService.ValidateUserAsync(user))
        {
            // Http 401 - lacks valid authentication credentials for the requested resource.
            // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/401
            return Unauthorized();
        }

        TokenDto token = await _service.AuthenticationService.CreateTokenAsync(populateExpiration: true);

        return Ok(token);
    }
}