using CRM.Core.Enums;

namespace CRM.Core.Exceptions
{
  public class CustomException : Exception
  {
    public ErrorTypes ErrorType { get; }

    public CustomException(
        ErrorTypes errorType,
        string message,
        Exception? innerException = null
      ) : base(message, innerException)
    {
      ErrorType = errorType;
    }

    public CustomException(
        ErrorTypes errorType,
        string message
      ) : base(message)
    {
      ErrorType = errorType;
    }
  }
}