using ElectroCorp.Platform.Shared.Domain.Model.Events;

namespace ElectroCorp.Platform.Billing.Domain.Model.Events;

public record SubscriptionCanceledEvent(int SubscriptionId) : IEvent;
