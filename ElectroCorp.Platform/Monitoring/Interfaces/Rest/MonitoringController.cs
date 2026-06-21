using System.Net.Mime;
using ElectroCorp.Platform.Monitoring.Application.CommandServices;
using ElectroCorp.Platform.Monitoring.Application.QueryServices;
using ElectroCorp.Platform.Monitoring.Domain.Model.Commands;
using ElectroCorp.Platform.Monitoring.Domain.Model.Queries;
using ElectroCorp.Platform.Monitoring.Interfaces.Rest.Resources;
using ElectroCorp.Platform.Monitoring.Interfaces.Rest.Transform;
using ElectroCorp.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using ElectroCorp.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElectroCorp.Platform.Monitoring.Interfaces.Rest;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[SwaggerTag("Available Energy Monitoring Endpoints.")]
public class MonitoringController(
    IEnergyReadingCommandService energyReadingCommandService,
    IEnergyReadingQueryService energyReadingQueryService,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost("readings")]
    [SwaggerOperation("Create Energy Reading", "Register a new energy consumption reading from a device.", OperationId = "CreateEnergyReading")]
    [SwaggerResponse(201, "The energy reading was successfully registered.", typeof(EnergyReadingResource))]
    [SwaggerResponse(400, "Invalid reading details.")]
    [SwaggerResponse(404, "Device not found.")]
    public async Task<IActionResult> CreateEnergyReading([FromBody] CreateEnergyReadingResource resource, CancellationToken cancellationToken)
    {
        var command = new CreateEnergyReadingCommand(resource.DeviceId, resource.ConsumptionValue);
        var result = await energyReadingCommandService.Handle(command, cancellationToken);

        return MonitoringActionResultAssembler.ToActionResult(
            this,
            result,
            problemDetailsFactory,
            reading => CreatedAtAction(nameof(GetReadingsByDeviceId), new { deviceId = reading.DeviceId }, EnergyReadingResourceFromEntityAssembler.ToResourceFromEntity(reading))
        );
    }

    [HttpGet("readings/{deviceId:int}")]
    [SwaggerOperation("Get Readings by Device Id", "List all energy readings registered for a specific device.", OperationId = "GetReadingsByDeviceId")]
    [SwaggerResponse(200, "List of energy readings retrieved.", typeof(IEnumerable<EnergyReadingResource>))]
    public async Task<IActionResult> GetReadingsByDeviceId(int deviceId, CancellationToken cancellationToken)
    {
        var query = new GetReadingsByDeviceIdQuery(deviceId);
        var readings = await energyReadingQueryService.Handle(query, cancellationToken);
        var resources = readings.Select(EnergyReadingResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("readings/{deviceId:int}/statistics")]
    [SwaggerOperation("Get Energy Statistics", "Get aggregate consumption statistics for a device over a period (Daily, Weekly, Monthly).", OperationId = "GetEnergyStatistics")]
    [SwaggerResponse(200, "Energy statistics retrieved.", typeof(EnergyStatisticsResource))]
    public async Task<IActionResult> GetEnergyStatistics(int deviceId, [FromQuery] string period = "daily", CancellationToken cancellationToken = default)
    {
        var query = new GetEnergyStatisticsQuery(deviceId, period);
        var readings = (await energyReadingQueryService.Handle(query, cancellationToken)).ToList();

        double total = readings.Sum(r => r.ConsumptionValue);
        double avg = readings.Count > 0 ? readings.Average(r => r.ConsumptionValue) : 0;

        var stats = new EnergyStatisticsResource(
            deviceId,
            period.ToLower(),
            total,
            avg,
            readings.Count
        );

        return Ok(stats);
    }
}
