using ElectroCorp.Platform.Shared.Domain.Model.Events;

namespace ElectroCorp.Platform.Devices.Domain.Model.Events;

public record DeviceCreatedEvent(int DeviceId, string Name, string Type, int OwnerId) : IEvent;
public record DeviceUpdatedEvent(int DeviceId, string Name, string Room) : IEvent;
public record DeviceStatusUpdatedEvent(int DeviceId, string Status) : IEvent;
public record DeviceToggledEvent(int DeviceId, string Status) : IEvent;
public record DeviceDeletedEvent(int DeviceId, int OwnerId, string Name) : IEvent;
