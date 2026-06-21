using ElectroCorp.Platform.Notifications.Domain.Model;
using ElectroCorp.Platform.Notifications.Domain.Model.Aggregates;
using ElectroCorp.Platform.Notifications.Interfaces.Rest.Resources;
using ElectroCorp.Platform.Shared.Application.Model;
using ElectroCorp.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;

namespace ElectroCorp.Platform.Notifications.Interfaces.Rest.Transform;

public static class NotificationsActionResultAssembler
{
    private static int ToStatusCodeFromNotificationError(NotificationError error)
    {
        return error switch
        {
            NotificationError.AlertNotFound => StatusCodes.Status404NotFound,
            NotificationError.UserNotFound => StatusCodes.Status404NotFound,
            NotificationError.OperationCancelled => StatusCodes.Status409Conflict,
            NotificationError.DatabaseError => StatusCodes.Status500InternalServerError,
            NotificationError.InternalServerError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };
    }

    public static IActionResult ToActionResult<T>(
        ControllerBase controller,
        Result<T> result,
        ProblemDetailsFactory problemDetailsFactory,
        Func<T, IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction(result.Value!);

        var statusCode = ToStatusCodeFromNotificationError((NotificationError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }
}

public static class AlertResourceFromEntityAssembler
{
    public static AlertResource ToResourceFromEntity(Alert entity)
    {
        return new AlertResource(
            entity.Id,
            entity.UserId,
            entity.Message,
            entity.Type,
            entity.IsRead,
            entity.Timestamp
        );
    }
}
