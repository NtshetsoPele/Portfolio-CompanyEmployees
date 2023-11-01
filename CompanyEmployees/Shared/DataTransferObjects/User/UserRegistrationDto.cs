namespace Shared.DataTransferObjects.User;

public class UserRegistrationDto
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }

    [Required(ErrorMessage = $"'{nameof(UserName)}' is required")]
    public string? UserName { get; init; }

    [Required(ErrorMessage = $"'{nameof(Password)}' is required")]
    public string? Password { get; init; }

    [Required(ErrorMessage = $"'{nameof(Email)}' is required")]
    [EmailAddress(ErrorMessage = $"'{nameof(Email)}' is invalid.")]
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; } // Should have its own validation.
    public ICollection<string>? Roles { get; init; }
}