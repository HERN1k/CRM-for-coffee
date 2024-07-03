using CRM.Infrastructure.Email.Services;

using RazorLight;

namespace CRM.API.Tools
{
  public static class RazorEngineBuilder
  {
    public static IRazorLightEngine CreateRazorLightEngine(IServiceProvider provider)
    {
      return new RazorLightEngineBuilder()
        .UseEmbeddedResourcesProject(
            typeof(EmailService).Assembly,
            "CRM.Infrastructure.Email.EmailModels"
        )
        .UseMemoryCachingProvider()
        .Build();
    }
  }
}