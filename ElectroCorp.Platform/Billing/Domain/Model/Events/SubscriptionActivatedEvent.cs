using ElectroCorp.Platform.Shared.Domain.Model.Events;

namespace ElectroCorp.Platform.Billing.Domain.Model.Events;

public record SubscriptionActivatedEvent(int SubscriptionId, int UserId, string PlanName) : IEvent;
