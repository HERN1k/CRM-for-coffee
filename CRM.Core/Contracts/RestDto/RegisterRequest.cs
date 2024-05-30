using System.ComponentModel.DataAnnotations;

namespace CRM.Core.Contracts.RestDto
{
    public record RegisterRequest(
        [Required] string firstName,
        [Required] string lastName,
        [Required] string fatherName,
        [Required] string email,
        [Required] string password,
        [Required] string post,
        [Required] int age,
        [Required] string gender,
        [Required] string phoneNumber
      );
}
