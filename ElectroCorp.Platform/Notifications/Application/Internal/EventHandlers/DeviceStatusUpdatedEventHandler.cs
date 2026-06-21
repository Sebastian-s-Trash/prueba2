using ElectroCorp.Platform.Devices.Domain.Model.Events;
using ElectroCorp.Platform.Devices.Application.QueryServices;
using ElectroCorp.Platform.Devices.Domain.Model.Queries;
using ElectroCorp.Platform.Notifications.Application.CommandServices;
using ElectroCorp.Platform.Notifications.Domain.Model.Commands;
using ElectroCorp.Platform.Shared.Application.Internal.EventHandlers;

namespace ElectroCorp.Platform.Notifications.Application.Internal.EventHandlers;

public class DeviceStatusUpdatedEventHandler(
    IAlertCommandService alertCommandService,
    IDeviceQueryService deviceQueryService)
    : IEventHandler<DeviceStatusUpdatedEvent>
{
    public async Task Handle(DeviceStatusUpdatedEvent domainEvent, CancellationToken cancellationToken)
    {
        var query = new GetDeviceByIdQuery(domainEvent.DeviceId);
        var device = await deviceQueryService.Handle(query, cancellationToken);
        if (device == null) return;

        var message = $"Device '{device.Name}' status has been updated to {domainEvent.Status}.";
        var command = new CreateAlertCommand(device.OwnerId, message, "Info");
        await alertCommandService.Handle(command, cancellationToken);
    }
}
