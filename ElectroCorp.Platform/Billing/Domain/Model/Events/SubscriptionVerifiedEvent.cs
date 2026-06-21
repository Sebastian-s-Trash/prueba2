using ElectroCorp.Platform.Shared.Domain.Model.Events;

namespace ElectroCorp.Platform.Billing.Domain.Model.Events;

public record SubscriptionVerifiedEvent(int SubscriptionId, string Status) : IEvent;
