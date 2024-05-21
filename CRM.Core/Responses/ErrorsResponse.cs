namespace CRM.Core.Responses
{
  public class ErrorsResponse(int code, string message)
  {
    public int ErrorCode { get; set; } = code;
    public string ErrorMessage { get; set; } = message;
  }
}
