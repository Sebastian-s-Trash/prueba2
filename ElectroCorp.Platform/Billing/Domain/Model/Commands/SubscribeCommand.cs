namespace ElectroCorp.Platform.Billing.Domain.Model.Commands;

public record SubscribeCommand(int UserId, string PlanName);
