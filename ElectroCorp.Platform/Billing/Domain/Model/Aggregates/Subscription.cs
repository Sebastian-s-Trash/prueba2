using ElectroCorp.Platform.Shared.Domain.Model.Entities;

namespace ElectroCorp.Platform.Billing.Domain.Model.Aggregates;

public enum SubscriptionStatus
{
    Active,
    Canceled,
    Expired
}

public class Subscription : IAuditableEntity
{
    public Subscription()
    {
        PlanName = string.Empty;
        Status = SubscriptionStatus.Active;
    }

    public Subscription(int userId, string planName)
    {
        UserId = userId;
        PlanName = planName;
        Status = SubscriptionStatus.Active;
        StartDate = DateTime.UtcNow;
        EndDate = DateTime.UtcNow.AddMonths(1); // 1 month plan
    }

    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string PlanName { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public Subscription Cancel()
    {
        Status = SubscriptionStatus.Canceled;
        EndDate = DateTime.UtcNow;
        return this;
    }

    public Subscription VerifyStatus()
    {
        if (Status == SubscriptionStatus.Active && DateTime.UtcNow > EndDate)
        {
            Status = SubscriptionStatus.Expired;
        }
        return this;
    }
}
