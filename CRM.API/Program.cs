using CRM.API.Helper;
using CRM.API.Middlewares;
using CRM.Application.Hubs;

using Microsoft.AspNetCore.CookiePolicy;

namespace CRM.API
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);
      var helper = new AppBuilder(builder);
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

      var app = builder.Build();
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

      app.UseMiddleware<LogFactoryMiddleware>();

      app.MapGraphQL("/Api/GraphQl");

      app.Run();
    }
  }
}