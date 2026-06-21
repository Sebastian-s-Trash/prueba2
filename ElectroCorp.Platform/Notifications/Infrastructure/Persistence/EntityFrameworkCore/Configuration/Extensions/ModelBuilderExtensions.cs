using ElectroCorp.Platform.Notifications.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace ElectroCorp.Platform.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyNotificationsConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Alert>().HasKey(a => a.Id);
        builder.Entity<Alert>().Property(a => a.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Alert>().Property(a => a.UserId).IsRequired();
        builder.Entity<Alert>().Property(a => a.Message).IsRequired();
        builder.Entity<Alert>().Property(a => a.Type).IsRequired();
        builder.Entity<Alert>().Property(a => a.IsRead).IsRequired();
        builder.Entity<Alert>().Property(a => a.Timestamp).IsRequired();
    }
}
