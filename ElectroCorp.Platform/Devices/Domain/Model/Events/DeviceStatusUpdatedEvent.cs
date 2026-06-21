using ElectroCorp.Platform.Shared.Domain.Model.Events;

namespace ElectroCorp.Platform.Devices.Domain.Model.Events;

public record DeviceStatusUpdatedEvent(int DeviceId, string Status) : IEvent;
