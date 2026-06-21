namespace ElectroCorp.Platform.Billing.Interfaces.Rest.Resources;

public record SubscribeResource(int UserId, string PlanName);
public record SubscriptionResource(int Id, int UserId, string PlanName, string Status, DateTime StartDate, DateTime EndDate);
public record ProcessPaymentResource(decimal Amount);
public record PaymentResource(int Id, int SubscriptionId, decimal Amount, DateTime PaymentDate, string Status);
