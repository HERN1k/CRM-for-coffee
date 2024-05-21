using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Contracts
{
  public record UpdatePasswordRequest(
      [Required] string email,
      [Required] string password,
      [Required] string newPassword
    );
}
