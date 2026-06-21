using ElectroCorp.Platform.Monitoring.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace ElectroCorp.Platform.Monitoring.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyMonitoringConfiguration(this ModelBuilder builder)
    {
        builder.Entity<EnergyReading>().HasKey(r => r.Id);
        builder.Entity<EnergyReading>().Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<EnergyReading>().Property(r => r.DeviceId).IsRequired();
        builder.Entity<EnergyReading>().Property(r => r.ConsumptionValue).IsRequired();
        builder.Entity<EnergyReading>().Property(r => r.Timestamp).IsRequired();
    }
}
