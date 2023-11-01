namespace Entities.Models;

public class Employee
{
    [Column(name: "EmployeeId")]
    public Guid Id { get; set; }

    [Required(ErrorMessage = $"'{nameof(Name)}' is a required field.")]
    [MaxLength(30, ErrorMessage = $"Maximum length for the '{nameof(Name)}' is 30 characters.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = $"'{nameof(Age)}' is a required field.")]
    public int Age { get; set; }

    [Required(ErrorMessage = $"'{nameof(Position)}' is a required field.")]
    [MaxLength(20, ErrorMessage = $"Maximum length for the '{nameof(Position)}' is 20 characters.")]
    public string? Position { get; set; }

    [ForeignKey(nameof(Company))]
    public Guid CompanyId { get; set; }

    public Company? Company { get; set; }
}