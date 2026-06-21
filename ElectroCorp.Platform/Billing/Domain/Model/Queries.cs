namespace ElectroCorp.Platform.Billing.Domain.Model.Queries;

public record GetSubscriptionByUserIdQuery(int UserId);
public record GetSubscriptionByIdQuery(int SubscriptionId);
public record GetPaymentsBySubscriptionIdQuery(int SubscriptionId);
public record GetPaymentByIdQuery(int PaymentId);
