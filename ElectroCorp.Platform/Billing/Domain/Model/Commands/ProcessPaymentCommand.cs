namespace ElectroCorp.Platform.Billing.Domain.Model.Commands;

public record ProcessPaymentCommand(int SubscriptionId, decimal Amount);
