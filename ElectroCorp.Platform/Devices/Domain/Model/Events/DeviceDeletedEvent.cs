using ElectroCorp.Platform.Shared.Domain.Model.Events;

namespace ElectroCorp.Platform.Devices.Domain.Model.Events;

public record DeviceDeletedEvent(int DeviceId, int OwnerId, string Name) : IEvent;
