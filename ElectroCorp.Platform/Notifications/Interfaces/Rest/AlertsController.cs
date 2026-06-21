using System.Net.Mime;
using ElectroCorp.Platform.Notifications.Application.CommandServices;
using ElectroCorp.Platform.Notifications.Application.QueryServices;
using ElectroCorp.Platform.Notifications.Domain.Model.Commands;
using ElectroCorp.Platform.Notifications.Domain.Model.Queries;
using ElectroCorp.Platform.Notifications.Interfaces.Rest.Resources;
using ElectroCorp.Platform.Notifications.Interfaces.Rest.Transform;
using ElectroCorp.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using ElectroCorp.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElectroCorp.Platform.Notifications.Interfaces.Rest;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[SwaggerTag("Available Alert & Notification Endpoints.")]
public class AlertsController(
    IAlertCommandService alertCommandService,
    IAlertQueryService alertQueryService,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost]
    [SwaggerOperation("Create Alert", "Generate a new in-app alert/notification for a user.", OperationId = "CreateAlert")]
    [SwaggerResponse(201, "The alert was successfully created.", typeof(AlertResource))]
    [SwaggerResponse(400, "Invalid alert details.")]
    [SwaggerResponse(404, "Target user not found.")]
    public async Task<IActionResult> CreateAlert([FromBody] CreateAlertResource resource, CancellationToken cancellationToken)
    {
        var command = new CreateAlertCommand(resource.UserId, resource.Message, resource.Type);
        var result = await alertCommandService.Handle(command, cancellationToken);

        return NotificationsActionResultAssembler.ToActionResult(
            this,
            result,
            problemDetailsFactory,
            alert => CreatedAtAction(nameof(GetAlertsByUserId), new { userId = alert.UserId }, AlertResourceFromEntityAssembler.ToResourceFromEntity(alert))
        );
    }

    [HttpGet("user/{userId:int}")]
    [SwaggerOperation("Get Alerts by User Id", "List all alerts registered for a specific user.", OperationId = "GetAlertsByUserId")]
    [SwaggerResponse(200, "List of alerts retrieved.", typeof(IEnumerable<AlertResource>))]
    public async Task<IActionResult> GetAlertsByUserId(int userId, CancellationToken cancellationToken)
    {
        var query = new GetAlertsByUserIdQuery(userId);
        var alerts = await alertQueryService.Handle(query, cancellationToken);
        var resources = alerts.Select(AlertResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpPost("{id:int}/read")]
    [SwaggerOperation("Mark Alert as Read", "Mark a specific alert as read.", OperationId = "MarkAlertAsRead")]
    [SwaggerResponse(200, "Alert marked as read.", typeof(AlertResource))]
    [SwaggerResponse(404, "Alert not found.")]
    public async Task<IActionResult> MarkAlertAsRead(int id, CancellationToken cancellationToken)
    {
        var command = new MarkAlertAsReadCommand(id);
        var result = await alertCommandService.Handle(command, cancellationToken);

        return NotificationsActionResultAssembler.ToActionResult(
            this,
            result,
            problemDetailsFactory,
            alert => Ok(AlertResourceFromEntityAssembler.ToResourceFromEntity(alert))
        );
    }
}
