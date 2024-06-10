namespace CRM.Core.Responses
{
  public class OperationResult(bool result)
  {
    public bool Success { get; set; } = result;
  }
}