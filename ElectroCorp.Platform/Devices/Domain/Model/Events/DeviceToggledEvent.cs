using ElectroCorp.Platform.Shared.Domain.Model.Events;

namespace ElectroCorp.Platform.Devices.Domain.Model.Events;

public record DeviceToggledEvent(int DeviceId, string Status) : IEvent;
