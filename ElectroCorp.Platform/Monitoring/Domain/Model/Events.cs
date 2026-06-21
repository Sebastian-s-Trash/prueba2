using ElectroCorp.Platform.Shared.Domain.Model.Events;

namespace ElectroCorp.Platform.Monitoring.Domain.Model.Events;

public record EnergyReadingCreatedEvent(int ReadingId, int DeviceId, double ConsumptionValue) : IEvent;
