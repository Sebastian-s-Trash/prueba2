using ElectroCorp.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using ElectroCorp.Platform.Billing.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using ElectroCorp.Platform.Devices.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using ElectroCorp.Platform.Monitoring.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using ElectroCorp.Platform.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using ElectroCorp.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using ElectroCorp.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace ElectroCorp.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     Application database context for the ElectroCorp Platform
/// </summary>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Apply configurations for each Bounded Context
        builder.ApplyIamConfiguration();
        builder.ApplyBillingConfiguration();
        builder.ApplyDevicesConfiguration();
        builder.ApplyMonitoringConfiguration();
        builder.ApplyNotificationsConfiguration();

        // General Naming Convention for the database objects (snake_case and plural)
        builder.UseSnakeCaseNamingConvention();
    }
}
