using ElectroCorp.Platform.Billing.Domain.Model.Aggregates;
using ElectroCorp.Platform.Billing.Domain.Repositories;
using ElectroCorp.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using ElectroCorp.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ElectroCorp.Platform.Billing.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class SubscriptionRepository(AppDbContext context) : BaseRepository<Subscription>(context), ISubscriptionRepository
{
    public async Task<Subscription?> FindByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        return await Context.Set<Subscription>()
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
    }

    public async Task<bool> ExistsByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        return await Context.Set<Subscription>()
            .AnyAsync(s => s.UserId == userId, cancellationToken);
    }
}
