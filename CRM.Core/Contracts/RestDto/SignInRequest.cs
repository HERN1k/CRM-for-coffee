using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Contracts.RestDto
{
    public record SignInRequest(
        [Required] string email,
        [Required] string password
      );
}
