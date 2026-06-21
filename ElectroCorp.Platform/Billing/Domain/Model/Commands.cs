namespace ElectroCorp.Platform.Billing.Domain.Model.Commands;

public record SubscribeCommand(int UserId, string PlanName);
public record CancelSubscriptionCommand(int SubscriptionId);
public record VerifySubscriptionCommand(int SubscriptionId);
public record ProcessPaymentCommand(int SubscriptionId, decimal Amount);
