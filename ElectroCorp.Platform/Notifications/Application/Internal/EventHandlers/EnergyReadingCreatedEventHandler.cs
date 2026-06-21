using ElectroCorp.Platform.Monitoring.Domain.Model.Events;
using ElectroCorp.Platform.Devices.Application.QueryServices;
using ElectroCorp.Platform.Devices.Domain.Model.Queries;
using ElectroCorp.Platform.Notifications.Application.CommandServices;
using ElectroCorp.Platform.Notifications.Domain.Model.Commands;
using ElectroCorp.Platform.Shared.Application.Internal.EventHandlers;

namespace ElectroCorp.Platform.Notifications.Application.Internal.EventHandlers;

public class EnergyReadingCreatedEventHandler(
    IAlertCommandService alertCommandService,
    IDeviceQueryService deviceQueryService)
    : IEventHandler<EnergyReadingCreatedEvent>
{
    private const double HighConsumptionThreshold = 10.0;

    public async Task Handle(EnergyReadingCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        var query = new GetDeviceByIdQuery(domainEvent.DeviceId);
        var device = await deviceQueryService.Handle(query, cancellationToken);
        if (device == null) return;

        // Create standard info log alert
        var infoMessage = $"New energy reading registered for '{device.Name}': {domainEvent.ConsumptionValue} kWh.";
        var infoCommand = new CreateAlertCommand(device.OwnerId, infoMessage, "Info");
        await alertCommandService.Handle(infoCommand, cancellationToken);

        // Raise warning if it exceeds threshold
        if (domainEvent.ConsumptionValue > HighConsumptionThreshold)
        {
            var warningMessage = $"[WARNING] High energy consumption detected on device '{device.Name}' in room '{device.Room}': {domainEvent.ConsumptionValue} kWh exceeds threshold!";
            var warningCommand = new CreateAlertCommand(device.OwnerId, warningMessage, "Warning");
            await alertCommandService.Handle(warningCommand, cancellationToken);
        }
    }
}
