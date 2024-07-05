using CRM.API.Tools;

namespace CRM.API
{
  public class Program
  {
    public static int QueryCount { get; set; } = 0; // Just for fun

    public static void Main(string[] args)
    {
      AppConfigurator.Logger
        .WithProperty("EventId", 0)
        .Info("Initialization...");

      try
      {
        var builder = WebApplication.CreateBuilder(args);

        AppConfigurator.ConfigureBuilder(builder);

        var app = builder.Build();

        AppConfigurator.ConfigureApplication(app);

        app.Run();
      }
      catch (Exception ex)
      {
        AppConfigurator.Logger
          .Error(ex, "Stopped program because of exception");

        throw;
      }
      finally
      {
        NLog.LogManager.Shutdown();
      }
    }
  }
}