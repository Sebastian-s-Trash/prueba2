using ElectroCorp.Platform.Notifications.Domain.Model.Aggregates;
using ElectroCorp.Platform.Notifications.Domain.Model.Queries;

namespace ElectroCorp.Platform.Notifications.Application.QueryServices;

public interface IAlertQueryService
{
    Task<Alert?> Handle(GetAlertByIdQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<Alert>> Handle(GetAlertsByUserIdQuery query, CancellationToken cancellationToken = default);
}
