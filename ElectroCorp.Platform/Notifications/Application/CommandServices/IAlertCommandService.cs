using ElectroCorp.Platform.Notifications.Domain.Model.Aggregates;
using ElectroCorp.Platform.Notifications.Domain.Model.Commands;
using ElectroCorp.Platform.Shared.Application.Model;

namespace ElectroCorp.Platform.Notifications.Application.CommandServices;

public interface IAlertCommandService
{
    Task<Result<Alert>> Handle(CreateAlertCommand command, CancellationToken cancellationToken = default);
    Task<Result<Alert>> Handle(MarkAlertAsReadCommand command, CancellationToken cancellationToken = default);
}
