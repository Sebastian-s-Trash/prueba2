using ElectroCorp.Platform.Devices.Domain.Model;
using ElectroCorp.Platform.Devices.Domain.Model.Aggregates;
using ElectroCorp.Platform.Devices.Interfaces.Rest.Resources;
using ElectroCorp.Platform.Shared.Application.Model;
using ElectroCorp.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;

namespace ElectroCorp.Platform.Devices.Interfaces.Rest.Transform;

public static class DevicesActionResultAssembler
{
    private static int ToStatusCodeFromDeviceError(DeviceError error)
    {
        return error switch
        {
            DeviceError.DeviceNotFound => StatusCodes.Status404NotFound,
            DeviceError.OwnerNotFound => StatusCodes.Status404NotFound,
            DeviceError.InvalidDeviceData => StatusCodes.Status400BadRequest,
            DeviceError.DeviceAlreadyRemoved => StatusCodes.Status409Conflict,
            DeviceError.OperationCancelled => StatusCodes.Status409Conflict,
            DeviceError.DatabaseError => StatusCodes.Status500InternalServerError,
            DeviceError.InternalServerError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };
    }

    public static IActionResult ToActionResult(
        ControllerBase controller,
        Result result,
        ProblemDetailsFactory problemDetailsFactory,
        Func<IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction();

        var statusCode = ToStatusCodeFromDeviceError((DeviceError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }

    public static IActionResult ToActionResult<T>(
        ControllerBase controller,
        Result<T> result,
        ProblemDetailsFactory problemDetailsFactory,
        Func<T, IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction(result.Value!);

        var statusCode = ToStatusCodeFromDeviceError((DeviceError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }
}

public static class DeviceResourceFromEntityAssembler
{
    public static DeviceResource ToResourceFromEntity(Device entity)
    {
        return new DeviceResource(
            entity.Id,
            entity.Name,
            entity.Type,
            entity.Status.ToString(),
            entity.OwnerId,
            entity.Room,
            entity.DateAdded
        );
    }
}
