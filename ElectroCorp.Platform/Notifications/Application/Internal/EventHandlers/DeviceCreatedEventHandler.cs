using ElectroCorp.Platform.Devices.Domain.Model.Events;
using ElectroCorp.Platform.Notifications.Application.CommandServices;
using ElectroCorp.Platform.Notifications.Domain.Model.Commands;
using ElectroCorp.Platform.Shared.Application.Internal.EventHandlers;

namespace ElectroCorp.Platform.Notifications.Application.Internal.EventHandlers;

public class DeviceCreatedEventHandler(IAlertCommandService alertCommandService) : IEventHandler<DeviceCreatedEvent>
{
    public async Task Handle(DeviceCreatedEvent domainEvent, CancellationToken cancellationToken)
    {
        var message = $"Device '{domainEvent.Name}' of type '{domainEvent.Type}' was registered successfully.";
        var command = new CreateAlertCommand(domainEvent.OwnerId, message, "Info");
        await alertCommandService.Handle(command, cancellationToken);
    }
}
