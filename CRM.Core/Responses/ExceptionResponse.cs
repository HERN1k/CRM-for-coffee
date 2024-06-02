namespace CRM.Core.Responses
{
  public class ExceptionResponse(string code, string message)
  {
    public string StatusCode { get; set; } = code;
    public string Message { get; set; } = message;
  }
}
