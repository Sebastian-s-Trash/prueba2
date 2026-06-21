using ElectroCorp.Platform.Shared.Domain.Model.Events;

namespace ElectroCorp.Platform.Devices.Domain.Model.Events;

public record DeviceCreatedEvent(int DeviceId, string Name, string Type, int OwnerId) : IEvent;
