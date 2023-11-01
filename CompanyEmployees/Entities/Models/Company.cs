namespace Entities.Models;

public class Company
{
    [Column(name: "CompanyId")]
    public Guid Id { get; set; }

    // Nullable, however validation won't allow.
    [Required(ErrorMessage = $"'{nameof(Name)}' is a required field.")]
    [MaxLength(60, ErrorMessage = $"Maximum length for '{nameof(Name)}' is 60 characters.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = $"'{nameof(Address)}' is a required field.")]
    [MaxLength(60, ErrorMessage = $"Maximum length for '{nameof(Address)}' is 60 characters")]
    public string? Address { get; set; }

    public string? Country { get; set; }

    public ICollection<Employee>? Employees { get; set; }
}