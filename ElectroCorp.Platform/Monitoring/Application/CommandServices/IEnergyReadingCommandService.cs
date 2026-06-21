using ElectroCorp.Platform.Monitoring.Domain.Model.Aggregates;
using ElectroCorp.Platform.Monitoring.Domain.Model.Commands;
using ElectroCorp.Platform.Shared.Application.Model;

namespace ElectroCorp.Platform.Monitoring.Application.CommandServices;

public interface IEnergyReadingCommandService
{
    Task<Result<EnergyReading>> Handle(CreateEnergyReadingCommand command, CancellationToken cancellationToken = default);
}
