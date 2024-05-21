using System.ComponentModel.DataAnnotations;

namespace CRM.API.Contarcts
{
  public record SignInRequest(
      [Required] string email,
      [Required] string password
    );
}
