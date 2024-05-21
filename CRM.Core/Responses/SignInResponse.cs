namespace CRM.Core.Responses
{
  public class SignInResponse
  {
    public string RefreshToken { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Post { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
  }
}
