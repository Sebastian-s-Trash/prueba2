using ElectroCorp.Platform.Notifications.Domain.Model.Aggregates;
using ElectroCorp.Platform.Shared.Domain.Repositories;

namespace ElectroCorp.Platform.Notifications.Domain.Repositories;

public interface IAlertRepository : IBaseRepository<Alert>
{
    Task<IEnumerable<Alert>> FindByUserIdAsync(int userId, CancellationToken cancellationToken);
}
