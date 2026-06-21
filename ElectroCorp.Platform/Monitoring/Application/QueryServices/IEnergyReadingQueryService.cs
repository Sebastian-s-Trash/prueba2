using ElectroCorp.Platform.Monitoring.Domain.Model.Aggregates;
using ElectroCorp.Platform.Monitoring.Domain.Model.Queries;

namespace ElectroCorp.Platform.Monitoring.Application.QueryServices;

public interface IEnergyReadingQueryService
{
    Task<IEnumerable<EnergyReading>> Handle(GetReadingsByDeviceIdQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<EnergyReading>> Handle(GetEnergyStatisticsQuery query, CancellationToken cancellationToken = default);
}
