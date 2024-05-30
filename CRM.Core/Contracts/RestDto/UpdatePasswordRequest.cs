using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Contracts.RestDto
{
    public record UpdatePasswordRequest(
        [Required] string email,
        [Required] string password,
        [Required] string newPassword
      );
}
