using ElectroCorp.Platform.Shared.Domain.Model.Events;

namespace ElectroCorp.Platform.Devices.Domain.Model.Events;

public record DeviceUpdatedEvent(int DeviceId, string Name, string Room) : IEvent;
