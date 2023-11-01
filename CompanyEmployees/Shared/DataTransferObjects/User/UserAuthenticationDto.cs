namespace Shared.DataTransferObjects.User;

public record UserAuthenticationDto
{
    [Required(ErrorMessage = $"'{nameof(UserName)}' is required.")]
    public string? UserName { get; init; }

    [Required(ErrorMessage = $"'{nameof(Password)}' is required.")]
    public string? Password { get; init; }
}