using ElectroCorp.Platform.Monitoring.Domain.Model.Aggregates;
using ElectroCorp.Platform.Monitoring.Domain.Repositories;
using ElectroCorp.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using ElectroCorp.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ElectroCorp.Platform.Monitoring.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class EnergyReadingRepository(AppDbContext context) : BaseRepository<EnergyReading>(context), IEnergyReadingRepository
{
    public async Task<IEnumerable<EnergyReading>> FindByDeviceIdAsync(int deviceId, CancellationToken cancellationToken)
    {
        return await Context.Set<EnergyReading>()
            .Where(r => r.DeviceId == deviceId)
            .OrderByDescending(r => r.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<EnergyReading>> FindByDeviceIdAndPeriodAsync(int deviceId, string period, CancellationToken cancellationToken)
    {
        var cutoff = period.ToLower() switch
        {
            "daily" => DateTime.UtcNow.AddDays(-1),
            "weekly" => DateTime.UtcNow.AddDays(-7),
            "monthly" => DateTime.UtcNow.AddDays(-30),
            _ => DateTime.UtcNow.AddDays(-1)
        };

        return await Context.Set<EnergyReading>()
            .Where(r => r.DeviceId == deviceId && r.Timestamp >= cutoff)
            .OrderBy(r => r.Timestamp)
            .ToListAsync(cancellationToken);
    }
}
