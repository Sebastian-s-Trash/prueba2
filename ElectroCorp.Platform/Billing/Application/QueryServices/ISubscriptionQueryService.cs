using ElectroCorp.Platform.Billing.Domain.Model.Aggregates;
using ElectroCorp.Platform.Billing.Domain.Model.Queries;

namespace ElectroCorp.Platform.Billing.Application.QueryServices;

public interface ISubscriptionQueryService
{
    Task<Subscription?> Handle(GetSubscriptionByIdQuery query, CancellationToken cancellationToken = default);
    Task<Subscription?> Handle(GetSubscriptionByUserIdQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> Handle(GetPaymentsBySubscriptionIdQuery query, CancellationToken cancellationToken = default);
    Task<Payment?> Handle(GetPaymentByIdQuery query, CancellationToken cancellationToken = default);
}
