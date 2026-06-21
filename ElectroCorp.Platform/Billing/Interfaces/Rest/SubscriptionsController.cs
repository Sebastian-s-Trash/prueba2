using System.Net.Mime;
using ElectroCorp.Platform.Billing.Application.CommandServices;
using ElectroCorp.Platform.Billing.Application.QueryServices;
using ElectroCorp.Platform.Billing.Domain.Model.Commands;
using ElectroCorp.Platform.Billing.Domain.Model.Queries;
using ElectroCorp.Platform.Billing.Interfaces.Rest.Resources;
using ElectroCorp.Platform.Billing.Interfaces.Rest.Transform;
using ElectroCorp.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using ElectroCorp.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElectroCorp.Platform.Billing.Interfaces.Rest;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[SwaggerTag("Available Billing & Subscription Endpoints.")]
public class SubscriptionsController(
    ISubscriptionCommandService subscriptionCommandService,
    ISubscriptionQueryService subscriptionQueryService,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost]
    [SwaggerOperation("Subscribe", "Create a new subscription plan.", OperationId = "Subscribe")]
    [SwaggerResponse(201, "The subscription was successfully activated.", typeof(SubscriptionResource))]
    [SwaggerResponse(400, "Invalid plan details.")]
    [SwaggerResponse(409, "Active subscription already exists.")]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeResource resource, CancellationToken cancellationToken)
    {
        var command = new SubscribeCommand(resource.UserId, resource.PlanName);
        var result = await subscriptionCommandService.Handle(command, cancellationToken);

        return BillingActionResultAssembler.ToActionResult(
            this,
            result,
            problemDetailsFactory,
            sub => CreatedAtAction(nameof(GetSubscriptionById), new { id = sub.Id }, SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(sub))
        );
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation("Get Subscription by Id", "Get subscription details by its ID.", OperationId = "GetSubscriptionById")]
    [SwaggerResponse(200, "Subscription details retrieved.", typeof(SubscriptionResource))]
    [SwaggerResponse(404, "Subscription not found.")]
    public async Task<IActionResult> GetSubscriptionById(int id, CancellationToken cancellationToken)
    {
        var query = new GetSubscriptionByIdQuery(id);
        var sub = await subscriptionQueryService.Handle(query, cancellationToken);

        if (sub == null) return NotFound("Subscription not found.");
        return Ok(SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(sub));
    }

    [HttpGet("user/{userId:int}")]
    [SwaggerOperation("Get Subscription by User Id", "Get subscription details for a specific user.", OperationId = "GetSubscriptionByUserId")]
    [SwaggerResponse(200, "Subscription details retrieved.", typeof(SubscriptionResource))]
    [SwaggerResponse(404, "Subscription not found.")]
    public async Task<IActionResult> GetSubscriptionByUserId(int userId, CancellationToken cancellationToken)
    {
        var query = new GetSubscriptionByUserIdQuery(userId);
        var sub = await subscriptionQueryService.Handle(query, cancellationToken);

        if (sub == null) return NotFound("Subscription not found.");
        return Ok(SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(sub));
    }

    [HttpPost("{id:int}/cancel")]
    [SwaggerOperation("Cancel Subscription", "Cancel an active subscription.", OperationId = "CancelSubscription")]
    [SwaggerResponse(200, "Subscription canceled.", typeof(SubscriptionResource))]
    [SwaggerResponse(404, "Subscription not found.")]
    public async Task<IActionResult> CancelSubscription(int id, CancellationToken cancellationToken)
    {
        var command = new CancelSubscriptionCommand(id);
        var result = await subscriptionCommandService.Handle(command, cancellationToken);

        return BillingActionResultAssembler.ToActionResult(
            this,
            result,
            problemDetailsFactory,
            sub => Ok(SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(sub))
        );
    }

    [HttpPost("{id:int}/verify")]
    [SwaggerOperation("Verify Subscription", "Verify if a subscription is active or has expired.", OperationId = "VerifySubscription")]
    [SwaggerResponse(200, "Subscription status verified.", typeof(SubscriptionResource))]
    [SwaggerResponse(404, "Subscription not found.")]
    public async Task<IActionResult> VerifySubscription(int id, CancellationToken cancellationToken)
    {
        var command = new VerifySubscriptionCommand(id);
        var result = await subscriptionCommandService.Handle(command, cancellationToken);

        return BillingActionResultAssembler.ToActionResult(
            this,
            result,
            problemDetailsFactory,
            sub => Ok(SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(sub))
        );
    }

    [HttpPost("{id:int}/payments")]
    [SwaggerOperation("Process Payment", "Process a subscription billing payment.", OperationId = "ProcessPayment")]
    [SwaggerResponse(200, "Payment processed successfully.", typeof(PaymentResource))]
    [SwaggerResponse(400, "Invalid payment details.")]
    [SwaggerResponse(404, "Subscription not found.")]
    public async Task<IActionResult> ProcessPayment(int id, [FromBody] ProcessPaymentResource resource, CancellationToken cancellationToken)
    {
        var command = new ProcessPaymentCommand(id, resource.Amount);
        var result = await subscriptionCommandService.Handle(command, cancellationToken);

        return BillingActionResultAssembler.ToActionResult(
            this,
            result,
            problemDetailsFactory,
            payment => Ok(PaymentResourceFromEntityAssembler.ToResourceFromEntity(payment))
        );
    }

    [HttpGet("{id:int}/payments")]
    [SwaggerOperation("Get Payments", "Get all payments for a specific subscription.", OperationId = "GetPayments")]
    [SwaggerResponse(200, "List of payments retrieved.", typeof(IEnumerable<PaymentResource>))]
    public async Task<IActionResult> GetPayments(int id, CancellationToken cancellationToken)
    {
        var query = new GetPaymentsBySubscriptionIdQuery(id);
        var payments = await subscriptionQueryService.Handle(query, cancellationToken);
        var resources = payments.Select(PaymentResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }
}
