using System.Security.Claims;
using System.Text;

using CRM.API.GraphQl.Mutations;
using CRM.API.GraphQl.Queries;
using CRM.Application.Security;
using CRM.Application.Services.AuthServices;
using CRM.Application.Services.OrderServices;
using CRM.Application.Services.ProductsServices;
using CRM.Application.Services.UserServices;
using CRM.Core.GraphQlTypes.OrderTypes;
using CRM.Core.GraphQlTypes.ProductTypes;
using CRM.Core.GraphQlTypes.UserTypes;
using CRM.Core.Interfaces.AuthServices;
using CRM.Core.Interfaces.Email;
using CRM.Core.Interfaces.JwtToken;
using CRM.Core.Interfaces.OrderServices;
using CRM.Core.Interfaces.PasswordHesher;
using CRM.Core.Interfaces.ProductsServices;
using CRM.Core.Interfaces.Repositories;
using CRM.Core.Interfaces.Settings;
using CRM.Core.Interfaces.UserServices;
using CRM.Data;
using CRM.Data.Repositories;
using CRM.Infrastructure.Email;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using NLog.Web;

using RazorLight;

namespace CRM.API.Helper
{
  public class AppBuilder(WebApplicationBuilder builder)
  {
    private readonly WebApplicationBuilder _builder = builder;

    #region Setting up Logger
    public void ConfigureLogger()
    {
      _builder.Logging.ClearProviders();
      _builder.Host.UseNLog();
    }
    #endregion

    #region Setting up CORS
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
    #endregion

    #region Setting up Base
    public void ConfigureBase()
    {
      _builder.Logging
        .AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning)
        .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);
      _builder.Services.AddControllers();
      _builder.Services.AddEndpointsApiExplorer();
    }
    #endregion

    #region Setting up Swagger
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
    #endregion

    #region Setting up Settings
    public void ConfigureSettings()
    {
      _builder.Services.Configure<HttpSettings>(
          _builder.Configuration.GetSection(nameof(HttpSettings))
        );
      _builder.Services.Configure<JwtSettings>(
          _builder.Configuration.GetSection(nameof(JwtSettings))
        );
      _builder.Services.Configure<EmailSettings>(
          _builder.Configuration.GetSection(nameof(EmailSettings))
        );
      _builder.Services.Configure<EmailConfirmRegisterSettings>(
          _builder.Configuration.GetSection(nameof(EmailConfirmRegisterSettings))
        );
      _builder.Services.Configure<BusinessInformationSettings>(
          _builder.Configuration.GetSection(nameof(BusinessInformationSettings))
        );
    }
    #endregion

    #region Setting up Authentication
    public void ConfigureAuthentication()
    {
      var jwtSettings = _builder.Configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
      if (jwtSettings == null)
      {
        Console.WriteLine("Error parse appsettings.json [ ConfigureServices() ].");
        jwtSettings = new JwtSettings();
      }
      _builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, (options) =>
        {
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.IssuerJwt,
            ValidateAudience = true,
            ValidAudience = jwtSettings.AudienceJwt,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecurityKeyJwt)
              ),
            ClockSkew = TimeSpan.FromMinutes(jwtSettings.ClockSkewJwt)
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
    #endregion

    #region Setting up Authorization
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
    #endregion

    #region Setting up Db
    public void ConfigureDb()
    {
      _builder.Services.AddPooledDbContextFactory<AppDBContext>(options =>
        options.UseNpgsql(
          _builder.Configuration.GetConnectionString("PostgresConnectionStrings"),
          conf => conf.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
        ));
    }
    #endregion

    #region Setting up SignalR
    public void ConfigureSignalR()
    {
      _builder.Services.AddSignalR((options) =>
      {
        options.EnableDetailedErrors = true;
        options.ClientTimeoutInterval = TimeSpan.FromMinutes(60);
      });
    }
    #endregion

    #region Setting up GraphQL
    public void ConfigureGraphQL()
    {
      _builder.Services.AddHttpContextAccessor();
      _builder.Services
        .AddGraphQLServer()
        .AddErrorFilter<GraphQlErrorFilter>()
        .RegisterDbContext<AppDBContext>(DbContextKind.Pooled)
        .AddAuthorization()
        .AddProjections()
        .AddFiltering()
        .AddSorting()
        .AddDefaultTransactionScopeHandler()
        .ModifyRequestOptions(options => options.IncludeExceptionDetails = true)
        .AddQueryType<Queries>()
        .AddMutationType<Mutations>()
        .AddType<UserType>()
        .AddType<ProductCategoryType>()
        .AddType<ProductType>()
        .AddType<AddOnType>()
        .AddType<OrderType>()
        .AddType<OrderProductType>()
        .AddType<OrderAddOnType>();
    }
    #endregion

    #region Setting up Di
    public void ConfigureDi()
    {
      _builder.Services.AddScoped<IRepository, Repository>();
      _builder.Services.AddScoped<IEmailService, EmailService>();

      _builder.Services.AddScoped<IHesherService, HesherService>();
      _builder.Services.AddScoped<ITokenService, TokenService>();

      _builder.Services.AddScoped<IRegisterService, RegisterService>();
      _builder.Services.AddScoped<ISignInService, SignInService>();
      _builder.Services.AddScoped<ISignOutService, SignOutService>();
      _builder.Services.AddScoped<IAuthSundryService, AuthSundryService>();
      _builder.Services.AddScoped<IAuthRecoveryService, AuthRecoveryService>();

      _builder.Services.AddScoped<IUserService, UserService>();
      _builder.Services.AddScoped<IProductsService, ProductsService>();
      _builder.Services.AddScoped<IOrderService, OrderService>();

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
    #endregion
  }
}