using ElectroCorp.Platform.Notifications.Domain.Model.Aggregates;
using ElectroCorp.Platform.Notifications.Domain.Repositories;
using ElectroCorp.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using ElectroCorp.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ElectroCorp.Platform.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class AlertRepository(AppDbContext context) : BaseRepository<Alert>(context), IAlertRepository
{
    public async Task<IEnumerable<Alert>> FindByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        return await Context.Set<Alert>()
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync(cancellationToken);
    }
}
