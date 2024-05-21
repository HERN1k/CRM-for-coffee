using LogLib.Types;

namespace CRM.API.Middlewares
{
  public class LogFactoryMiddleware
  {
    private readonly RequestDelegate _next;

    public LogFactoryMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ILoggerLib logger)
    {
      logger.WriteLog(context.Request.Method);
      await _next(context);
    }
  }
}
