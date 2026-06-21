using ElectroCorp.Platform.Monitoring.Application.QueryServices;
using ElectroCorp.Platform.Monitoring.Domain.Model.Aggregates;
using ElectroCorp.Platform.Monitoring.Domain.Model.Queries;
using ElectroCorp.Platform.Monitoring.Domain.Repositories;

namespace ElectroCorp.Platform.Monitoring.Application.Internal.QueryServices;

public class EnergyReadingQueryService(IEnergyReadingRepository energyReadingRepository) : IEnergyReadingQueryService
{
    public async Task<IEnumerable<EnergyReading>> Handle(GetReadingsByDeviceIdQuery query, CancellationToken cancellationToken)
    {
        return await energyReadingRepository.FindByDeviceIdAsync(query.DeviceId, cancellationToken);
    }

    public async Task<IEnumerable<EnergyReading>> Handle(GetEnergyStatisticsQuery query, CancellationToken cancellationToken)
    {
        return await energyReadingRepository.FindByDeviceIdAndPeriodAsync(query.DeviceId, query.Period, cancellationToken);
    }
}
