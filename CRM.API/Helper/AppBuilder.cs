using System.Security.Claims;
using System.Text;

using CRM.Application.GraphQl.Queries;
using CRM.Application.Security;
using CRM.Application.Services;
using CRM.Application.Types;
using CRM.Application.Types.Options;
using CRM.Data;
using CRM.Data.Stores;
using CRM.Data.Types;
using CRM.Infrastructure.Email;

using LogLib;
using LogLib.Types;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using RazorLight;

namespace CRM.API.Helper
{
  public class AppBuilder(
      WebApplicationBuilder builder
      )
  {
    private readonly WebApplicationBuilder _builder = builder;

    public void ConfigureCORS()
    {
      _builder.Services.AddCors(options =>
      {
        options.AddPolicy(name: "mainCors", policy =>
        {
          policy.WithOrigins(
              "https://dreamworkout.pp.ua",
              "http://dreamworkout.pp.ua"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        });
      });
    }

    public void ConfigureBase()
    {
      _builder.Logging
        .AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning)
        .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
      _builder.Services.AddControllers();
      _builder.Services.AddEndpointsApiExplorer();
    }

    public void ConfigureSwagger()
    {
      _builder.Services.AddSwaggerGen((options) =>
      {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
          Title = "CRM for you!",
          Version = "0.0.1",
          Description = "CRM for your cafe ≧◠‿◠≦✌"
        });
        options.EnableAnnotations();
      });
    }

    public void ConfigureOptions()
    {
      _builder.Services.Configure<HttpOptions>(
          _builder.Configuration.GetSection(nameof(HttpOptions))
        );
      _builder.Services.Configure<JwtOptions>(
          _builder.Configuration.GetSection(nameof(JwtOptions))
        );
      _builder.Services.Configure<EmailOptions>(
          _builder.Configuration.GetSection(nameof(EmailOptions))
        );
      _builder.Services.Configure<EmailConfirmRegisterOptions>(
          _builder.Configuration.GetSection(nameof(EmailConfirmRegisterOptions))
        );
    }

    public void ConfigureAuthentication()
    {
      var jwtOptions = _builder.Configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
      if (jwtOptions == null)
      {
        Console.WriteLine("Error parse appsettings.json [ ConfigureServices() ].");
        jwtOptions = new JwtOptions();
      }
      _builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, (options) =>
        {
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.IssuerJwt,
            ValidateAudience = true,
            ValidAudience = jwtOptions.AudienceJwt,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.SecurityKeyJwt)
              ),
            ClockSkew = TimeSpan.FromMinutes(jwtOptions.ClockSkewJwt)
          };

          options.Events = new JwtBearerEvents
          {
            OnMessageReceived = (context) =>
            {
              context.Token = context.Request.Cookies["accessToken"];
              return Task.CompletedTask;
            }
          };
        });
    }

    public void ConfigureAuthorization()
    {
      _builder.Services.AddAuthorization((options) =>
      {
        options.AddPolicy("Admin", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
        options.AddPolicy("Manager", policy => policy.RequireClaim(ClaimTypes.Role, "Manager"));
        options.AddPolicy("Worker", policy => policy.RequireClaim(ClaimTypes.Role, "Worker"));

        options.AddPolicy("AdminOrLower", (policy) => policy.RequireAssertion((context) =>
            context.User.HasClaim((u) => u.Type == ClaimTypes.Role &&
            (u.Value == "Admin" || u.Value == "Manager" || u.Value == "Worker"))
        ));
        options.AddPolicy("ManagerOrLower", (policy) => policy.RequireAssertion((context) =>
            context.User.HasClaim((u) => u.Type == ClaimTypes.Role &&
            (u.Value == "Manager" || u.Value == "Worker"))
        ));

        options.AddPolicy("ManagerOrUpper", (policy) => policy.RequireAssertion((context) =>
            context.User.HasClaim((u) => u.Type == ClaimTypes.Role &&
            (u.Value == "Admin" || u.Value == "Manager"))
        ));
      });
    }

    public void ConfigureDb()
    {
      _builder.Services.AddDbContext<AppDBContext>((options) =>
      {
        options.UseNpgsql(_builder.Configuration.GetConnectionString("PostgresConnectionStrings"));
        //.EnableSensitiveDataLogging()
        //.UseLoggerFactory(LoggerFactory.Create(builder => { builder.AddConsole(); }));
      }, ServiceLifetime.Scoped);
    }

    public void ConfigureSignalR()
    {
      _builder.Services.AddSignalR((options) =>
      {
        options.EnableDetailedErrors = true;
        options.ClientTimeoutInterval = TimeSpan.FromMinutes(60);
      });
    }

    public void ConfigureGraphQL()
    {
      _builder.Services
        .AddGraphQLServer()
        .AddAuthorization()
        .AddProjections()
        .AddFiltering()
        .AddSorting()
        .AddQueryType<ChackoutQueries>();
    }

    public void ConfigureDi()
    {
      _builder.Services.AddScoped<IHesherService, HesherService>();
      _builder.Services.AddScoped<ITokenService, TokenService>();
      _builder.Services.AddScoped<IEmailService, EmailService>();

      _builder.Services.AddScoped<IRegisterService, RegisterService>();
      _builder.Services.AddScoped<ISignInService, SignInService>();
      _builder.Services.AddScoped<ISignOutService, SignOutService>();
      _builder.Services.AddScoped<IAuthSundryService, AuthSundryService>();
      _builder.Services.AddScoped<IAuthRecoveryService, AuthRecoveryService>();

      _builder.Services.AddScoped<IRegisterStore, RegisterStore>();
      _builder.Services.AddScoped<ISignInStore, SignInStore>();
      _builder.Services.AddScoped<ISignOutStore, SignOutStore>();
      _builder.Services.AddScoped<IAuthSundryStore, AuthSundryStore>();
      _builder.Services.AddScoped<IAuthRecoveryStore, AuthRecoveryStore>();

      _builder.Services.AddSingleton<ILoggerLib, LoggerLib>();
      _builder.Services.AddSingleton<IRazorLightEngine>((provider) =>
      {
        return new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(
                typeof(EmailService).Assembly,
                "CRM.Infrastructure.Email.EmailModels"
              )
            .UseMemoryCachingProvider()
            .Build();
      });
    }
  }
}