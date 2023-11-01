namespace Shared.DataTransferObjects.Employee.Request;

// Id property not needed because it will be accepted through the URI.
public abstract record EmployeeForManipulationDto
{
    [Required(ErrorMessage = $"Employee '{nameof(Name)}' is a required field.")]
    [MaxLength(30, ErrorMessage = $"Maximum length for '{nameof(Name)}' is 30 characters.")]
    public string? Name { get; init; }

    [Range(18, int.MaxValue, ErrorMessage = $"'{nameof(Age)}' is required and it can't be lower than 18.")]
    public int Age { get; init; }

    [Required(ErrorMessage = $"'{nameof(Position)}' is a required field.")]
    [MaxLength(20, ErrorMessage = $"Maximum length for '{nameof(Position)}' is 20 characters.")]
    public string? Position { get; init; }
}