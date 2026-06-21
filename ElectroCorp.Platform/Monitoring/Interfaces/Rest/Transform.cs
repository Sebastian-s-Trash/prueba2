using ElectroCorp.Platform.Monitoring.Domain.Model;
using ElectroCorp.Platform.Monitoring.Domain.Model.Aggregates;
using ElectroCorp.Platform.Monitoring.Interfaces.Rest.Resources;
using ElectroCorp.Platform.Shared.Application.Model;
using ElectroCorp.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;

namespace ElectroCorp.Platform.Monitoring.Interfaces.Rest.Transform;

public static class MonitoringActionResultAssembler
{
    private static int ToStatusCodeFromMonitoringError(MonitoringError error)
    {
        return error switch
        {
            MonitoringError.ReadingNotFound => StatusCodes.Status404NotFound,
            MonitoringError.DeviceNotFound => StatusCodes.Status404NotFound,
            MonitoringError.InvalidReadingValue => StatusCodes.Status400BadRequest,
            MonitoringError.OperationCancelled => StatusCodes.Status409Conflict,
            MonitoringError.DatabaseError => StatusCodes.Status500InternalServerError,
            MonitoringError.InternalServerError => StatusCodes.Status500InternalServerError,
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

        var statusCode = ToStatusCodeFromMonitoringError((MonitoringError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }
}

public static class EnergyReadingResourceFromEntityAssembler
{
    public static EnergyReadingResource ToResourceFromEntity(EnergyReading entity)
    {
        return new EnergyReadingResource(
            entity.Id,
            entity.DeviceId,
            entity.ConsumptionValue,
            entity.Timestamp
        );
    }
}
