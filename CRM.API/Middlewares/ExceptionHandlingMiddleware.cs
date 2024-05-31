using CRM.Core.Enums;
using CRM.Core.Exceptions;

namespace CRM.API.Middlewares
{
  public class ExceptionHandlingMiddleware
  {
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(
        RequestDelegate next
      )
    {
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        await _next(context);
      }
      catch (CustomException ex)
      {
        await HandleCustomExceptionAsync(context, ex);
      }
      catch (Exception ex)
      {
        await HandleExceptionAsync(context, ex);
      }
    }

    private static Task HandleCustomExceptionAsync(HttpContext context, CustomException exception)
    {
      context.Response.ContentType = "application/json";

      string statusCode;
      string message = exception.Message;

      switch (exception.ErrorType)
      {
        case ErrorTypes.ServerError:
          context.Response.StatusCode = 500;
          statusCode = ErrorTypes.ServerError.GetValue();
          break;
        case ErrorTypes.ValidationError:
          context.Response.StatusCode = 400;
          statusCode = ErrorTypes.ValidationError.GetValue();
          break;
        case ErrorTypes.BadRequest:
          context.Response.StatusCode = 400;
          statusCode = ErrorTypes.BadRequest.GetValue();
          break;
        case ErrorTypes.Unauthorized:
          context.Response.StatusCode = 401;
          statusCode = ErrorTypes.Unauthorized.GetValue();
          break;
        default:
          context.Response.StatusCode = 500;
          statusCode = ErrorTypes.ServerError.GetValue();
          break;
      }

      var response = new
      {
        StatusCode = statusCode,
        Message = message,
      };

      return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
      context.Response.ContentType = "application/json";
      context.Response.StatusCode = 500;

      string statusCode = "SERVER_ERROR";
      string message = exception.Message;

      if (exception.InnerException != null)
        message = exception.InnerException.Message;

      var response = new
      {
        StatusCode = statusCode,
        Message = message,
      };

      return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
  }
}