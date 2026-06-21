using ElectroCorp.Platform.Billing.Domain.Model.Aggregates;
using ElectroCorp.Platform.Shared.Domain.Repositories;

namespace ElectroCorp.Platform.Billing.Domain.Repositories;

public interface ISubscriptionRepository : IBaseRepository<Subscription>
{
    Task<Subscription?> FindByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task<bool> ExistsByUserIdAsync(int userId, CancellationToken cancellationToken);
}
