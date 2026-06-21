using ElectroCorp.Platform.Notifications.Application.CommandServices;
using ElectroCorp.Platform.Notifications.Domain.Model;
using ElectroCorp.Platform.Notifications.Domain.Model.Aggregates;
using ElectroCorp.Platform.Notifications.Domain.Model.Commands;
using ElectroCorp.Platform.Notifications.Domain.Repositories;
using ElectroCorp.Platform.Iam.Interfaces.Acl;
using ElectroCorp.Platform.Resources.Errors;
using ElectroCorp.Platform.Shared.Application.Model;
using ElectroCorp.Platform.Shared.Domain.Repositories;
using Microsoft.Extensions.Localization;

namespace ElectroCorp.Platform.Notifications.Application.Internal.CommandServices;

public class AlertCommandService(
    IAlertRepository alertRepository,
    IIamContextFacade iamContextFacade,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IAlertCommandService
{
    private readonly IStringLocalizer<ErrorMessages> _localizer = localizer;

    public async Task<Result<Alert>> Handle(CreateAlertCommand command, CancellationToken cancellationToken)
    {
        var username = await iamContextFacade.FetchUsernameByUserId(command.UserId, cancellationToken);
        if (string.IsNullOrEmpty(username))
            return Result<Alert>.Failure(NotificationError.UserNotFound,
                _localizer[$"{nameof(NotificationError)}.{nameof(NotificationError.UserNotFound)}"]);

        var alert = new Alert(command.UserId, command.Message, command.Type);
        try
        {
            await alertRepository.AddAsync(alert, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Alert>.Success(alert);
        }
        catch (Exception)
        {
            return Result<Alert>.Failure(NotificationError.DatabaseError, _localizer[$"{nameof(NotificationError)}.{nameof(NotificationError.DatabaseError)}"]);
        }
    }

    public async Task<Result<Alert>> Handle(MarkAlertAsReadCommand command, CancellationToken cancellationToken)
    {
        var alert = await alertRepository.FindByIdAsync(command.AlertId, cancellationToken);
        if (alert == null)
            return Result<Alert>.Failure(NotificationError.AlertNotFound,
                _localizer[$"{nameof(NotificationError)}.{nameof(NotificationError.AlertNotFound)}"]);

        alert.MarkAsRead();
        try
        {
            alertRepository.Update(alert);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Alert>.Success(alert);
        }
        catch (Exception)
        {
            return Result<Alert>.Failure(NotificationError.DatabaseError, _localizer[$"{nameof(NotificationError)}.{nameof(NotificationError.DatabaseError)}"]);
        }
    }
}
