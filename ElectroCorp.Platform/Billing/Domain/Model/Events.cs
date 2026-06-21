using ElectroCorp.Platform.Shared.Domain.Model.Events;

namespace ElectroCorp.Platform.Billing.Domain.Model.Events;

public record SubscriptionActivatedEvent(int SubscriptionId, int UserId, string PlanName) : IEvent;
public record SubscriptionCanceledEvent(int SubscriptionId) : IEvent;
public record SubscriptionVerifiedEvent(int SubscriptionId, string Status) : IEvent;
public record PaymentRegisteredEvent(int PaymentId, int SubscriptionId, decimal Amount) : IEvent;
