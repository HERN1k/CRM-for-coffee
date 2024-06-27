using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Contracts.RestDto
{
  public record RecoveryPasswordRequest(
    [Required] string Email,
    [Required] string Post,
    [Required] string PhoneNumber
  );
}
