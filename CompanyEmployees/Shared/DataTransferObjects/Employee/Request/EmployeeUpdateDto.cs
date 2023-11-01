namespace Shared.DataTransferObjects.Employee.Request;

// Looks like the DTO for creation, but they're conceptually different:
//   updation vs creation.
public record EmployeeUpdateDto : EmployeeForManipulationDto;