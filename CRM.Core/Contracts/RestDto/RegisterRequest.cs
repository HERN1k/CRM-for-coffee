using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Contracts.RestDto
{
  public record RegisterRequest(
    [Required] string FirstName,
    [Required] string LastName,
    [Required] string FatherName,
    [Required] string Email,
    [Required] string Password,
    [Required] string Post,
    [Required] int Age,
    [Required] string Gender,
    [Required] string PhoneNumber
  );
}
