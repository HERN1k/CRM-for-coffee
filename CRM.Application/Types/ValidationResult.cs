
namespace CRM.Application.Types
{
  public class ValidationResult
  {
    public bool IsSuccess { get; set; } = false;
    public string Field { get; set; } = string.Empty;
  }
}
