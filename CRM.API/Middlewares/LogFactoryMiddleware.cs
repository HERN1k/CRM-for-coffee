namespace CRM.API.Middlewares
{
  public class LogFactoryMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly ILogger<LogFactoryMiddleware> _logger;

    public LogFactoryMiddleware(
        RequestDelegate next,
        ILogger<LogFactoryMiddleware> logger
      )
    {
      _next = next;
      _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      Program.QueryCount += 1;
      string logString = $"{context.Request.Method}\tCount: {Program.QueryCount}";
      _logger.LogInformation(logString);
      await _next(context);
    }
  }
}
