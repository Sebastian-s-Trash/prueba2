namespace ElectroCorp.Platform.Billing.Domain.Model;

public enum BillingError
{
    SubscriptionNotFound,
    PaymentNotFound,
    SubscriptionAlreadyActive,
    InvalidPlan,
    DatabaseError,
    OperationCancelled,
    InternalServerError
}
