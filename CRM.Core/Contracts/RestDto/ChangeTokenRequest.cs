using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Contracts.RestDto
{
  public record ChangeTokenRequest(
    [Required] string Email
  );
}
