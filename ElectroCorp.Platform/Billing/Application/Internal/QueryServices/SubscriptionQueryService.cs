using ElectroCorp.Platform.Billing.Application.QueryServices;
using ElectroCorp.Platform.Billing.Domain.Model.Aggregates;
using ElectroCorp.Platform.Billing.Domain.Model.Queries;
using ElectroCorp.Platform.Billing.Domain.Repositories;

namespace ElectroCorp.Platform.Billing.Application.Internal.QueryServices;

public class SubscriptionQueryService(
    ISubscriptionRepository subscriptionRepository,
    IPaymentRepository paymentRepository)
    : ISubscriptionQueryService
{
    public async Task<Subscription?> Handle(GetSubscriptionByIdQuery query, CancellationToken cancellationToken)
    {
        return await subscriptionRepository.FindByIdAsync(query.SubscriptionId, cancellationToken);
    }

    public async Task<Subscription?> Handle(GetSubscriptionByUserIdQuery query, CancellationToken cancellationToken)
    {
        return await subscriptionRepository.FindByUserIdAsync(query.UserId, cancellationToken);
    }

    public async Task<IEnumerable<Payment>> Handle(GetPaymentsBySubscriptionIdQuery query, CancellationToken cancellationToken)
    {
        return await paymentRepository.FindBySubscriptionIdAsync(query.SubscriptionId, cancellationToken);
    }

    public async Task<Payment?> Handle(GetPaymentByIdQuery query, CancellationToken cancellationToken)
    {
        return await paymentRepository.FindByIdAsync(query.PaymentId, cancellationToken);
    }
}
