using ElectroCorp.Platform.Iam.Application.Acl;
using ElectroCorp.Platform.Iam.Application.CommandServices;
using ElectroCorp.Platform.Iam.Application.Internal.CommandServices;
using ElectroCorp.Platform.Iam.Application.Internal.OutboundServices;
using ElectroCorp.Platform.Iam.Application.Internal.QueryServices;
using ElectroCorp.Platform.Iam.Application.QueryServices;
using ElectroCorp.Platform.Iam.Domain.Repositories;
using ElectroCorp.Platform.Iam.Infrastructure.Hashing.BCrypt.Services;
using ElectroCorp.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using ElectroCorp.Platform.Iam.Infrastructure.Pipeline.Middleware.Extensions;
using ElectroCorp.Platform.Iam.Infrastructure.Tokens.Jwt.Configuration;
using ElectroCorp.Platform.Iam.Infrastructure.Tokens.Jwt.Services;
using ElectroCorp.Platform.Iam.Interfaces.Acl;

using ElectroCorp.Platform.Billing.Application.CommandServices;
using ElectroCorp.Platform.Billing.Application.Internal.CommandServices;
using ElectroCorp.Platform.Billing.Application.Internal.QueryServices;
using ElectroCorp.Platform.Billing.Application.QueryServices;
using ElectroCorp.Platform.Billing.Domain.Repositories;
using ElectroCorp.Platform.Billing.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

using ElectroCorp.Platform.Devices.Application.CommandServices;
using ElectroCorp.Platform.Devices.Application.Internal.CommandServices;
using ElectroCorp.Platform.Devices.Application.Internal.QueryServices;
using ElectroCorp.Platform.Devices.Application.QueryServices;
using ElectroCorp.Platform.Devices.Domain.Repositories;
using ElectroCorp.Platform.Devices.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

using ElectroCorp.Platform.Monitoring.Application.CommandServices;
using ElectroCorp.Platform.Monitoring.Application.Internal.CommandServices;
using ElectroCorp.Platform.Monitoring.Application.Internal.QueryServices;
using ElectroCorp.Platform.Monitoring.Application.QueryServices;
using ElectroCorp.Platform.Monitoring.Domain.Repositories;
using ElectroCorp.Platform.Monitoring.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

using ElectroCorp.Platform.Notifications.Application.CommandServices;
using ElectroCorp.Platform.Notifications.Application.Internal.CommandServices;
using ElectroCorp.Platform.Notifications.Application.Internal.QueryServices;
using ElectroCorp.Platform.Notifications.Application.QueryServices;
using ElectroCorp.Platform.Notifications.Domain.Repositories;
using ElectroCorp.Platform.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

using ElectroCorp.Platform.Shared.Domain.Repositories;
using ElectroCorp.Platform.Shared.Infrastructure.Interfaces.AspNetCore.Configuration;
using ElectroCorp.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using ElectroCorp.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using ElectroCorp.Platform.Shared.Infrastructure.Pipeline.Middleware.Extensions;
using ElectroCorp.Platform.Shared.Infrastructure.i18n;

using Cortex.Mediator.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi;

using ProblemDetailsFactory = ElectroCorp.Platform.Shared.Interfaces.Rest.ProblemDetails.ProblemDetailsFactory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()));

// Add ProblemDetails services
builder.Services.AddProblemDetails();

// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy",
        policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Configure Database Connection
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionStringTemplate))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    var connectionString = Environment.ExpandEnvironmentVariables(connectionStringTemplate);
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    options.UseMySQL(connectionString)
        .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
        .EnableDetailedErrors();

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

// Register String Localizer using our dictionary-based implementation
builder.Services.AddSingleton(typeof(IStringLocalizer<>), typeof(SimpleStringLocalizer<>));

// Register the custom ProblemDetailsFactory
builder.Services.AddSingleton<ProblemDetailsFactory>();

// Swagger/OpenAPI Configuration
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "ElectroCorp.Platform",
            Version = "v1",
            Description = "ElectroCorp Smart Home Monitoring API",
            Contact = new OpenApiContact
            {
                Name = "ElectroCorp Dev Team",
                Email = "support@electrocorp.com"
            }
        });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT token (Bearer <token>)",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
    options.EnableAnnotations();
});

// Dependency Injection Setup

// Shared Kernel
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// IAM Context
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();

// Billing Context
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ISubscriptionCommandService, SubscriptionCommandService>();
builder.Services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();

// Devices Context
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<IDeviceCommandService, DeviceCommandService>();
builder.Services.AddScoped<IDeviceQueryService, DeviceQueryService>();

// Monitoring Context
builder.Services.AddScoped<IEnergyReadingRepository, EnergyReadingRepository>();
builder.Services.AddScoped<IEnergyReadingCommandService, EnergyReadingCommandService>();
builder.Services.AddScoped<IEnergyReadingQueryService, EnergyReadingQueryService>();

// Notifications Context
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<IAlertCommandService, AlertCommandService>();
builder.Services.AddScoped<IAlertQueryService, AlertQueryService>();

// Add Cortex Mediator for Event Handling and Bounded Context Decoupling
builder.Services.AddCortexMediator([typeof(Program)]);

var app = builder.Build();

// Apply pending migrations on startup (safe to call even when schema is up to date)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

// Global Exception Handler
app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply CORS Policy (Allows frontend integration)
app.UseCors("AllowAllPolicy");

app.UseRouting();

// Custom Request Authorization Middleware (IAM Bounded Context)
app.UseRequestAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
