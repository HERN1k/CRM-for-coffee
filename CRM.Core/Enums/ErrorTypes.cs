namespace CRM.Core.Enums
{
  public enum ErrorTypes
  {
    ServerError = 1,
    BadRequest = 2,
    ValidationError = 3,
    Unauthorized = 4,
    InvalidOperationException = 5,
    MailKitException = 6
  }

  public static class MyEnumExtensions
  {
    private static readonly Dictionary<ErrorTypes, string> stringValues = new Dictionary<ErrorTypes, string>
    {
      { ErrorTypes.ServerError, "SERVER_ERROR" },
      { ErrorTypes.BadRequest, "BAD_REQUEST" },
      { ErrorTypes.ValidationError, "VALIDATION_ERROR" },
      { ErrorTypes.Unauthorized, "UNAUTHORIZED" },
      { ErrorTypes.InvalidOperationException, "INVALID_OPERATION_EXCEPTION" },
      { ErrorTypes.MailKitException, "MAILKIT_EXCEPTION" }
    };

    public static string GetValue(this ErrorTypes value)
    {
      return stringValues[value];
    }
  }
}