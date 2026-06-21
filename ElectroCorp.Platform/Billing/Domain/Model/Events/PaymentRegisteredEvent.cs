using ElectroCorp.Platform.Shared.Domain.Model.Events;

namespace ElectroCorp.Platform.Billing.Domain.Model.Events;

public record PaymentRegisteredEvent(int PaymentId, int SubscriptionId, decimal Amount) : IEvent;
