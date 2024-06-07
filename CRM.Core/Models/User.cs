using CRM.Core.Models.BaseModels;

namespace CRM.Core.Models
{
  public class User : BaseModel
  {
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Post { get; set; } = string.Empty;
    public int Age { get; set; } = 0;
    public string Gender { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool IsConfirmed { get; set; }
    public string RegistrationDate { get; set; } = string.Empty;
  }
}
