using ElectroCorp.Platform.Shared.Domain.Model.Entities;

namespace ElectroCorp.Platform.Billing.Domain.Model.Aggregates;

public enum PaymentStatus
{
    Success,
    Failed
}

public class Payment : IAuditableEntity
{
    public Payment()
    {
        Status = PaymentStatus.Success;
    }

    public Payment(int subscriptionId, decimal amount, PaymentStatus status)
    {
        SubscriptionId = subscriptionId;
        Amount = amount;
        PaymentDate = DateTime.UtcNow;
        Status = status;
    }

    public int Id { get; private set; }
    public int SubscriptionId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime PaymentDate { get; private set; }
    public PaymentStatus Status { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
