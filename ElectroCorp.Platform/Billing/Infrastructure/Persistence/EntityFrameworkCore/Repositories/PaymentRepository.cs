using ElectroCorp.Platform.Billing.Domain.Model.Aggregates;
using ElectroCorp.Platform.Billing.Domain.Repositories;
using ElectroCorp.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using ElectroCorp.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ElectroCorp.Platform.Billing.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class PaymentRepository(AppDbContext context) : BaseRepository<Payment>(context), IPaymentRepository
{
    public async Task<IEnumerable<Payment>> FindBySubscriptionIdAsync(int subscriptionId, CancellationToken cancellationToken)
    {
        return await Context.Set<Payment>()
            .Where(p => p.SubscriptionId == subscriptionId)
            .ToListAsync(cancellationToken);
    }
}
