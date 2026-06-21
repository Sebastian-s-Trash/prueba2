using ElectroCorp.Platform.Billing.Domain.Model;
using ElectroCorp.Platform.Billing.Domain.Model.Aggregates;
using ElectroCorp.Platform.Billing.Interfaces.Rest.Resources;
using ElectroCorp.Platform.Shared.Application.Model;
using ElectroCorp.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;

namespace ElectroCorp.Platform.Billing.Interfaces.Rest.Transform;

public static class BillingActionResultAssembler
{
    private static int ToStatusCodeFromBillingError(BillingError error)
    {
        return error switch
        {
            BillingError.SubscriptionNotFound => StatusCodes.Status404NotFound,
            BillingError.PaymentNotFound => StatusCodes.Status404NotFound,
            BillingError.SubscriptionAlreadyActive => StatusCodes.Status409Conflict,
            BillingError.InvalidPlan => StatusCodes.Status400BadRequest,
            BillingError.OperationCancelled => StatusCodes.Status409Conflict,
            BillingError.DatabaseError => StatusCodes.Status500InternalServerError,
            BillingError.InternalServerError => StatusCodes.Status500InternalServerError,
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

        var statusCode = ToStatusCodeFromBillingError((BillingError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }
}

public static class SubscriptionResourceFromEntityAssembler
{
    public static SubscriptionResource ToResourceFromEntity(Subscription entity)
    {
        return new SubscriptionResource(
            entity.Id,
            entity.UserId,
            entity.PlanName,
            entity.Status.ToString(),
            entity.StartDate,
            entity.EndDate
        );
    }
}

public static class PaymentResourceFromEntityAssembler
{
    public static PaymentResource ToResourceFromEntity(Payment entity)
    {
        return new PaymentResource(
            entity.Id,
            entity.SubscriptionId,
            entity.Amount,
            entity.PaymentDate,
            entity.Status.ToString()
        );
    }
}
