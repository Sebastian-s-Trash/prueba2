using ElectroCorp.Platform.Devices.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace ElectroCorp.Platform.Devices.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyDevicesConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Device>().HasKey(d => d.Id);
        builder.Entity<Device>().Property(d => d.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Device>().Property(d => d.Name).IsRequired();
        builder.Entity<Device>().Property(d => d.Type).IsRequired();
        builder.Entity<Device>().Property(d => d.Status).HasConversion<string>().IsRequired();
        builder.Entity<Device>().Property(d => d.OwnerId).IsRequired();
        builder.Entity<Device>().Property(d => d.Room).IsRequired();
        builder.Entity<Device>().Property(d => d.DateAdded).IsRequired();
    }
}
