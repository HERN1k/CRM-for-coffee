using System.Security.Claims;
using System.Text;

using CRM.API.GraphQl.Mutations;
using CRM.API.GraphQl.Queries;
using CRM.API.Middlewares;
using CRM.Application.Hubs.Chackout;
using CRM.Application.Services.AuthServices.AuthRecovery;
using CRM.Application.Services.AuthServices.AuthSundry;
using CRM.Application.Services.AuthServices.Registration;
using CRM.Application.Services.AuthServices.SignIn;
using CRM.Application.Services.AuthServices.SignOut;
using CRM.Application.Services.BLogicServices.OrderServices;
using CRM.Application.Services.BLogicServices.ProductsServices;
using CRM.Application.Services.OrderServices;
using CRM.Application.Services.ProductsServices;
using CRM.Application.Services.ReportsServices.ExcelReports;
using CRM.Application.Services.UserServices.Users;
using CRM.Application.Tools.Security;
using CRM.Core.GraphQlTypes.OrderTypes;
using CRM.Core.GraphQlTypes.ProductTypes;
using CRM.Core.GraphQlTypes.UserTypes;
using CRM.Core.Interfaces.Infrastructure.Email;
using CRM.Core.Interfaces.Infrastructure.Excel;
using CRM.Core.Interfaces.Repositories.AuthRepositories.AuthRecovery;
using CRM.Core.Interfaces.Repositories.AuthRepositories.AuthSundry;
using CRM.Core.Interfaces.Repositories.AuthRepositories.Registration;
using CRM.Core.Interfaces.Repositories.AuthRepositories.SignIn;
using CRM.Core.Interfaces.Repositories.AuthRepositories.SignOut;
using CRM.Core.Interfaces.Repositories.Base;
using CRM.Core.Interfaces.Repositories.BLogicRepositories.OrderRepository;
using CRM.Core.Interfaces.Repositories.BLogicRepositories.Products;
using CRM.Core.Interfaces.Repositories.Excel;
using CRM.Core.Interfaces.Repositories.UserRepositories;
using CRM.Core.Interfaces.Services.AuthServices.AuthRecovery;
using CRM.Core.Interfaces.Services.AuthServices.AuthSundry;
using CRM.Core.Interfaces.Services.AuthServices.Registration;
using CRM.Core.Interfaces.Services.AuthServices.SignIn;
using CRM.Core.Interfaces.Services.AuthServices.SignOut;
using CRM.Core.Interfaces.Services.BLogicServices.OrderServices;
using CRM.Core.Interfaces.Services.BLogicServices.ProductsServices;
using CRM.Core.Interfaces.Services.ReportsServices.ExcelReports;
using CRM.Core.Interfaces.Services.UserServices;
using CRM.Core.Interfaces.Settings;
using CRM.Core.Interfaces.Tools.Security.JwtToken;
using CRM.Core.Mapper;
using CRM.Core.Mapper.Orders;
using CRM.Data.Contexts.ApplicationContext;
using CRM.Data.Repositories.AuthRepositories.AuthRecovery;
using CRM.Data.Repositories.AuthRepositories.AuthSundry;
using CRM.Data.Repositories.AuthRepositories.Registration;
using CRM.Data.Repositories.AuthRepositories.SignIn;
using CRM.Data.Repositories.AuthRepositories.SignOut;
using CRM.Data.Repositories.Base;
using CRM.Data.Repositories.BLogicRepositories.OrderRepository;
using CRM.Data.Repositories.BLogicRepositories.Products;
using CRM.Data.Repositories.Excel;
using CRM.Data.Repositories.UserRepositories;
using CRM.Infrastructure.Email.Components;
using CRM.Infrastructure.Email.Services;
using CRM.Infrastructure.Excel.Components;
using CRM.Infrastructure.Excel.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using NLog;
using NLog.Web;

namespace CRM.API.Tools
{
  /// <summary>
  /// Provides configuration methods for the web application.
  /// </summary>
  public static class AppConfigurator
  {
    private static WebApplicationBuilder? _builder { get; set; }
    private static WebApplication? _app { get; set; }
    public static Logger Logger { get; }

    /// <summary>
    /// Static constructor to initialize the logger for the AppConfigurator class.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if unable to determine the parent directory.</exception>
    static AppConfigurator()
    {
      var currentDirectory = Directory.GetCurrentDirectory();
      var parentDirectory = (Directory.GetParent(currentDirectory)?.FullName)
        ?? throw new InvalidOperationException("Unable to determine the parent directory.");

      string nlogConfigFilePath = System.IO.Path.Combine(parentDirectory, "CRM.API", "nlog.config");

      if (!File.Exists(nlogConfigFilePath))
        throw new FileNotFoundException(nameof(nlogConfigFilePath));

      Logger = LogManager.Setup()
        .LoadConfigurationFromFile(nlogConfigFilePath)
        .GetCurrentClassLogger();
    }

    /// <summary>
    /// Configures the application builder with various services and settings.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder to configure.</param>
    /// <exception cref="ArgumentNullException">Thrown if the builder is null.</exception>
    public static void ConfigureBuilder(WebApplicationBuilder builder)
    {
      _builder = builder
        ?? throw new ArgumentNullException(nameof(builder));

      ConfigureLogger();
      ConfigureCORS();
      ConfigureBase();
      ConfigureSwagger();
      ConfigureSettings();
      ConfigureAuthentication();
      ConfigureAuthorization();
      ConfigureDb();
      ConfigureSignalR();
      ConfigureGraphQL();
      ConfigureMapper();
      ConfigureDi();
    }

    /// <summary>
    /// Configures the web application by calling various configuration methods.
    /// </summary>
    /// <param name="app">The WebApplication to configure.</param>
    /// <exception cref="ArgumentNullException">Thrown if the application is null.</exception>
    public static void ConfigureApplication(WebApplication app)
    {
      _app = app
        ?? throw new ArgumentNullException(nameof(app));

      ConfigureSwaggerForDevelopment();
      ConfigureSecurityPolicies();
      ConfigureCookiePolicy();
      ConfigureHttpsAndAuthentication();
      ConfigureEndpointMappings();
      ConfigureCustomMiddlewares();
      RegisterStartupTasks();
    }

    /// <summary>
    /// Configures the logging providers for the application.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the builder is null.</exception>
    private static void ConfigureLogger()
    {
      ArgumentNullException.ThrowIfNull(_builder);

      _builder.Logging.ClearProviders();
      _builder.Host.UseNLog();
    }

    /// <summary>
    /// Configures CORS policies for the application.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the builder is null.</exception>
    private static void ConfigureCORS()
    {
      ArgumentNullException.ThrowIfNull(_builder);

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

    /// <summary>
    /// Configures basic services and settings for the application.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the builder is null.</exception>
    private static void ConfigureBase()
    {
      ArgumentNullException.ThrowIfNull(_builder);

      _builder.Logging
        .AddFilter("Microsoft.EntityFrameworkCore.Database.Command", Microsoft.Extensions.Logging.LogLevel.Warning)
        .AddFilter("Microsoft.EntityFrameworkCore", Microsoft.Extensions.Logging.LogLevel.Warning);
      _builder.Services.AddControllers();
      _builder.Services.AddEndpointsApiExplorer();
    }

    /// <summary>
    /// Configures Swagger for API documentation.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the builder is null.</exception>
    private static void ConfigureSwagger()
    {
      ArgumentNullException.ThrowIfNull(_builder);

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

    /// <summary>
    /// Configures various application settings from configuration.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the builder is null.</exception>
    private static void ConfigureSettings()
    {
      ArgumentNullException.ThrowIfNull(_builder);

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

    /// <summary>
    /// Configures JWT authentication for the application.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the builder is null.</exception>
    private static void ConfigureAuthentication()
    {
      ArgumentNullException.ThrowIfNull(_builder);

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

    /// <summary>
    /// Configures authorization policies for the application.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the builder is null.</exception>
    private static void ConfigureAuthorization()
    {
      ArgumentNullException.ThrowIfNull(_builder);

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

    /// <summary>
    /// Configures the database context for the application.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the builder is null.</exception>
    private static void ConfigureDb()
    {
      ArgumentNullException.ThrowIfNull(_builder);

      _builder.Services.AddPooledDbContextFactory<AppDBContext>(options =>
        options.UseNpgsql(
          _builder.Configuration.GetConnectionString("PostgresConnectionStrings"),
          conf => conf.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
        )
        //.EnableSensitiveDataLogging()
        //.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
        );
    }

    /// <summary>
    /// Configures SignalR for the application.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the builder is null.</exception>
    private static void ConfigureSignalR()
    {
      ArgumentNullException.ThrowIfNull(_builder);

      _builder.Services.AddSignalR((options) =>
      {
        options.EnableDetailedErrors = true;
        options.ClientTimeoutInterval = TimeSpan.FromMinutes(60);
      });
    }

    /// <summary>
    /// Configures GraphQL for the application.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the builder is null.</exception>
    private static void ConfigureGraphQL()
    {
      ArgumentNullException.ThrowIfNull(_builder);

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

    /// <summary>
    /// Configures AutoMapper for the application.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the builder is null.</exception>
    private static void ConfigureMapper()
    {
      ArgumentNullException.ThrowIfNull(_builder);

      _builder.Services.AddAutoMapper(config => config.AddProfile<MapperProfile>());
    }

    /// <summary>
    /// Configures dependency injection for the application.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the builder is null.</exception>
    private static void ConfigureDi()
    {
      ArgumentNullException.ThrowIfNull(_builder);

      // Base database repository
      _builder.Services.AddScoped<IBaseRepository, BaseRepository>();

      // Creation, decryption and verification of jwt tokens
      _builder.Services.AddScoped<ITokenService, TokenService>();

      // Email services
      _builder.Services.AddScoped<IEmailSender, EmailSender>();
      _builder.Services.AddScoped<IEmailService, EmailService>();

      // Configure html page render engine
      _builder.Services.AddSingleton(RazorEngineBuilder.CreateRazorLightEngine);

      // Excel services
      _builder.Services.AddScoped<IExcelService, ExcelService>();
      _builder.Services.AddScoped<IExcelRepository, ExcelRepository>();
      _builder.Services.AddScoped<IExcelStyles, ExcelStyles>();
      _builder.Services.AddScoped<IExcelFillingSheet, ExcelFillingSheet>();
      _builder.Services.AddScoped<IExcelSheetDataFiller, ExcelSheetDataFiller>();

      // Custom mapper converters
      _builder.Services.AddScoped<ToEntityOrderMapper>();
      _builder.Services.AddScoped<ToOrderMapper>();

      // HTTP request: Registration
      _builder.Services.AddScoped<IRegistrationService, RegistrationService>();
      _builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
      _builder.Services.AddScoped<IRegistrationComponents, RegistrationComponents>();

      // HTTP request: Sign in
      _builder.Services.AddScoped<ISignInService, SignInService>();
      _builder.Services.AddScoped<ISignInRepository, SignInRepository>();
      _builder.Services.AddScoped<ISignInComponents, SignInComponents>();

      // HTTP request: Sign out
      _builder.Services.AddScoped<ISignOutService, SignOutService>();
      _builder.Services.AddScoped<ISignOutRepository, SignOutRepository>();
      _builder.Services.AddScoped<ISignOutComponents, SignOutComponents>();

      // HTTP request: Auth sundry
      _builder.Services.AddScoped<IAuthSundryService, AuthSundryService>();
      _builder.Services.AddScoped<IAuthSundryRepository, AuthSundryRepository>();
      _builder.Services.AddScoped<IAuthSundryComponents, AuthSundryComponents>();

      // HTTP request: Auth recovery
      _builder.Services.AddScoped<IAuthRecoveryService, AuthRecoveryService>();
      _builder.Services.AddScoped<IAuthRecoveryRepository, AuthRecoveryRepository>();
      _builder.Services.AddScoped<IAuthRecoveryComponents, AuthRecoveryComponents>();

      // HTTP request: Excel reports
      _builder.Services.AddScoped<IExcelReportsService, ExcelReportsService>();
      _builder.Services.AddScoped<IExcelReportsComponents, ExcelReportsComponents>();

      // GraphGL request: order
      _builder.Services.AddScoped<IOrderService, OrderService>();
      _builder.Services.AddScoped<IOrderRepository, OrderRepository>();
      _builder.Services.AddScoped<IOrderComponents, OrderComponents>();

      // GraphGL request: order
      _builder.Services.AddScoped<IProductsService, ProductsService>();
      _builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
      _builder.Services.AddScoped<IProductsComponents, ProductsComponents>();

      // GraphGL request: user
      _builder.Services.AddScoped<IUserService, UserService>();
      _builder.Services.AddScoped<IUserRepository, UserRepository>();
    }

    /// <summary>
    /// Configures Swagger for use in the development environment.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the application builder is null.</exception>
    private static void ConfigureSwaggerForDevelopment()
    {
      ArgumentNullException.ThrowIfNull(_app);

      if (_app.Environment.IsDevelopment())
      {
        _app.UseSwagger();
        _app.UseSwaggerUI();
      }
    }

    /// <summary>
    /// Configures HSTS and CORS policies for the application.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the application builder is null.</exception>
    private static void ConfigureSecurityPolicies()
    {
      ArgumentNullException.ThrowIfNull(_app);

      _app.UseHsts();
      _app.UseCors("mainCors");
    }

    /// <summary>
    /// Configures the cookie policy for the application to ensure cookies are secure and have appropriate same-site and HTTP-only settings.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the application builder is null.</exception>
    private static void ConfigureCookiePolicy()
    {
      ArgumentNullException.ThrowIfNull(_app);

      _app.UseCookiePolicy(new CookiePolicyOptions
      {
        HttpOnly = HttpOnlyPolicy.Always,
        MinimumSameSitePolicy = SameSiteMode.Strict,
        Secure = CookieSecurePolicy.Always
      });
    }

    /// <summary>
    /// Configures HTTPS redirection, authentication, and authorization for the application.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the application builder is null.</exception>
    private static void ConfigureHttpsAndAuthentication()
    {
      ArgumentNullException.ThrowIfNull(_app);

      _app.UseHttpsRedirection();
      _app.UseAuthentication();
      _app.UseAuthorization();
    }

    /// <summary>
    /// Configures the endpoints for controllers, SignalR hubs, and GraphQL.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the application builder is null.</exception>
    private static void ConfigureEndpointMappings()
    {
      ArgumentNullException.ThrowIfNull(_app);

      _app.MapControllers();

      _app.MapHub<ChackoutHub>("/api/chackout_hub");
      _app.MapGraphQL("/api/graph_ql");
    }

    /// <summary>
    /// Configures custom middlewares for logging and exception handling.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the application builder is null.</exception>
    private static void ConfigureCustomMiddlewares()
    {
      ArgumentNullException.ThrowIfNull(_app);

      _app.UseMiddleware<LogFactoryMiddleware>();
      _app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    /// <summary>
    /// Registers tasks to be executed when the application starts.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if the application or logger is null.</exception>
    private static void RegisterStartupTasks()
    {
      ArgumentNullException.ThrowIfNull(_app);

      _app.Lifetime.ApplicationStarted.Register(() =>
      {
        // Создать скрипт для создания сущностей в базе данных при первом запуске

        Logger.WithProperty("EventId", 0)
          .Info("Initialization complete");
      });
    } // To Do
  }
}