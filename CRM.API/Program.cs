using CRM.API.Helper;
using CRM.API.Middlewares;
using CRM.Application.Hubs;

using Microsoft.AspNetCore.CookiePolicy;

using NLog;
using NLog.Web;

namespace CRM.API
{
  public class Program
  {
    public static int QueryCount { get; set; } = 0; // Just for fun

    public static void Main(string[] args)
    {
      #region Logger
      var logger = LogManager.Setup()
        .LoadConfigurationFromAppSettings()
        .GetCurrentClassLogger();
      logger.WithProperty("EventId", 0)
        .Info("Initialization...");
      #endregion

      try
      {
        var builder = WebApplication.CreateBuilder(args);
        var helper = new AppBuilder(builder);

        #region Builder configuration
        helper.ConfigureLogger();
        helper.ConfigureCORS();
        helper.ConfigureBase();
        helper.ConfigureSwagger();
        helper.ConfigureOptions();
        helper.ConfigureAuthentication();
        helper.ConfigureAuthorization();
        helper.ConfigureDb();
        helper.ConfigureSignalR();
        helper.ConfigureGraphQL();
        helper.ConfigureDi();
        #endregion

        var app = builder.Build();

        #region Middlewares
        if (app.Environment.IsDevelopment())
        {
          app.UseSwagger();
          app.UseSwaggerUI();
        }

        app.UseCors("mainCors");

        app.UseCookiePolicy(new CookiePolicyOptions
        {
          HttpOnly = HttpOnlyPolicy.Always,
          MinimumSameSitePolicy = SameSiteMode.Strict,
          Secure = CookieSecurePolicy.Always
        });
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.MapHub<ChackoutHub>("/Api/ChackoutHub");
        app.MapGraphQL("/Api/GraphQl");

        app.UseMiddleware<LogFactoryMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        #endregion

        #region Logger
        app.Lifetime.ApplicationStarted.Register(() =>
        {
          logger.WithProperty("EventId", 0)
            .Info("Initialization complete");
          logger.WithProperty("EventId", 0)
            .Info(string.Empty);
        });
        #endregion

        app.Run();
      }
      catch (Exception ex)
      {
        logger.Error(ex, "Stopped program because of exception");
        throw;
      }
      finally
      {
        LogManager.Shutdown();
      }
    }
  }
}