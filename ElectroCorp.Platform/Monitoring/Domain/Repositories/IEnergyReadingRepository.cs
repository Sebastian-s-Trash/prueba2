using ElectroCorp.Platform.Monitoring.Domain.Model.Aggregates;
using ElectroCorp.Platform.Shared.Domain.Repositories;

namespace ElectroCorp.Platform.Monitoring.Domain.Repositories;

public interface IEnergyReadingRepository : IBaseRepository<EnergyReading>
{
    Task<IEnumerable<EnergyReading>> FindByDeviceIdAsync(int deviceId, CancellationToken cancellationToken);
    Task<IEnumerable<EnergyReading>> FindByDeviceIdAndPeriodAsync(int deviceId, string period, CancellationToken cancellationToken);
}
