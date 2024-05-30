using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Contracts.RestDto
{
    public record RecoveryPasswordRequest(
        [Required] string email,
        [Required] string post,
        [Required] string phoneNumber
      );
}
