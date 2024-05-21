using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Contracts
{
  public record ChangeTokenRequest(
      [Required] string refreshToken,
      [Required] string email
    );
}
