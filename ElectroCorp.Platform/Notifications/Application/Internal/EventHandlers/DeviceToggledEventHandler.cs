using ElectroCorp.Platform.Devices.Domain.Model.Events;
using ElectroCorp.Platform.Devices.Application.QueryServices;
using ElectroCorp.Platform.Devices.Domain.Model.Queries;
using ElectroCorp.Platform.Notifications.Application.CommandServices;
using ElectroCorp.Platform.Notifications.Domain.Model.Commands;
using ElectroCorp.Platform.Shared.Application.Internal.EventHandlers;

namespace ElectroCorp.Platform.Notifications.Application.Internal.EventHandlers;

public class DeviceToggledEventHandler(
    IAlertCommandService alertCommandService,
    IDeviceQueryService deviceQueryService)
    : IEventHandler<DeviceToggledEvent>
{
    public async Task Handle(DeviceToggledEvent domainEvent, CancellationToken cancellationToken)
    {
        var query = new GetDeviceByIdQuery(domainEvent.DeviceId);
        var device = await deviceQueryService.Handle(query, cancellationToken);
        if (device == null) return;

        var message = $"Device '{device.Name}' in room '{device.Room}' was toggled {domainEvent.Status}.";
        var command = new CreateAlertCommand(device.OwnerId, message, "Info");
        await alertCommandService.Handle(command, cancellationToken);
    }
}
