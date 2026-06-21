using ElectroCorp.Platform.Devices.Domain.Model.Events;
using ElectroCorp.Platform.Notifications.Application.CommandServices;
using ElectroCorp.Platform.Notifications.Domain.Model.Commands;
using ElectroCorp.Platform.Shared.Application.Internal.EventHandlers;

namespace ElectroCorp.Platform.Notifications.Application.Internal.EventHandlers;

public class DeviceDeletedEventHandler(IAlertCommandService alertCommandService) : IEventHandler<DeviceDeletedEvent>
{
    public async Task Handle(DeviceDeletedEvent domainEvent, CancellationToken cancellationToken)
    {
        var message = $"Device '{domainEvent.Name}' (ID: {domainEvent.DeviceId}) was removed/deleted from the system.";
        var command = new CreateAlertCommand(domainEvent.OwnerId, message, "Info");
        await alertCommandService.Handle(command, cancellationToken);
    }
}
