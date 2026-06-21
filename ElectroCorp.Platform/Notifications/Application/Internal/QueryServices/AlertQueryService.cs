using ElectroCorp.Platform.Notifications.Application.QueryServices;
using ElectroCorp.Platform.Notifications.Domain.Model.Aggregates;
using ElectroCorp.Platform.Notifications.Domain.Model.Queries;
using ElectroCorp.Platform.Notifications.Domain.Repositories;

namespace ElectroCorp.Platform.Notifications.Application.Internal.QueryServices;

public class AlertQueryService(IAlertRepository alertRepository) : IAlertQueryService
{
    public async Task<Alert?> Handle(GetAlertByIdQuery query, CancellationToken cancellationToken)
    {
        return await alertRepository.FindByIdAsync(query.AlertId, cancellationToken);
    }

    public async Task<IEnumerable<Alert>> Handle(GetAlertsByUserIdQuery query, CancellationToken cancellationToken)
    {
        return await alertRepository.FindByUserIdAsync(query.UserId, cancellationToken);
    }
}
