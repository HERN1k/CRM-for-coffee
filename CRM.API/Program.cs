using CRM.API.Helper;
using CRM.API.Middlewares;

using Microsoft.AspNetCore.CookiePolicy;

namespace CRM.API
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);
      var helper = new AppBuilder(builder);
      helper.ConfigureBase();
      helper.ConfigureSwagger();
      helper.ConfigureOptions();
      helper.ConfigureAuthentication();
      helper.ConfigureAuthorization();
      helper.ConfigureDb();
      helper.ConfigureDi();

      var app = builder.Build();
      if (app.Environment.IsDevelopment())
      {
        app.UseSwagger();
        app.UseSwaggerUI();
      }
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

      app.UseMiddleware<LogFactoryMiddleware>();

      app.Run();
    }
  }
}
