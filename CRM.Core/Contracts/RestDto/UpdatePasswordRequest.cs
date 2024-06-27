using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Contracts.RestDto
{
  public record UpdatePasswordRequest(
    [Required] string Email,
    [Required] string Password,
    [Required] string NewPassword
  );
}
