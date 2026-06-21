using ElectroCorp.Platform.Billing.Domain.Model.Aggregates;
using ElectroCorp.Platform.Shared.Domain.Repositories;

namespace ElectroCorp.Platform.Billing.Domain.Repositories;

public interface IPaymentRepository : IBaseRepository<Payment>
{
    Task<IEnumerable<Payment>> FindBySubscriptionIdAsync(int subscriptionId, CancellationToken cancellationToken);
}
