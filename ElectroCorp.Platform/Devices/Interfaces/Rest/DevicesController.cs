using System.Net.Mime;
using ElectroCorp.Platform.Devices.Application.CommandServices;
using ElectroCorp.Platform.Devices.Application.QueryServices;
using ElectroCorp.Platform.Devices.Domain.Model.Aggregates;
using ElectroCorp.Platform.Devices.Domain.Model.Commands;
using ElectroCorp.Platform.Devices.Domain.Model.Queries;
using ElectroCorp.Platform.Devices.Interfaces.Rest.Resources;
using ElectroCorp.Platform.Devices.Interfaces.Rest.Transform;
using ElectroCorp.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using ElectroCorp.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElectroCorp.Platform.Devices.Interfaces.Rest;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[SwaggerTag("Available Device Monitoring & Control Endpoints.")]
public class DevicesController(
    IDeviceCommandService deviceCommandService,
    IDeviceQueryService deviceQueryService,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost]
    [SwaggerOperation("Create Device", "Register a new smart home device.", OperationId = "CreateDevice")]
    [SwaggerResponse(201, "The device was successfully registered.", typeof(DeviceResource))]
    [SwaggerResponse(400, "Invalid device data.")]
    [SwaggerResponse(404, "Owner user not found.")]
    public async Task<IActionResult> CreateDevice([FromBody] CreateDeviceResource resource, CancellationToken cancellationToken)
    {
        var command = new CreateDeviceCommand(resource.Name, resource.Type, resource.OwnerId, resource.Room);
        var result = await deviceCommandService.Handle(command, cancellationToken);

        return DevicesActionResultAssembler.ToActionResult(
            this,
            result,
            problemDetailsFactory,
            device => CreatedAtAction(nameof(GetDeviceById), new { id = device.Id }, DeviceResourceFromEntityAssembler.ToResourceFromEntity(device))
        );
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation("Get Device by Id", "Get device details by its ID.", OperationId = "GetDeviceById")]
    [SwaggerResponse(200, "Device details retrieved.", typeof(DeviceResource))]
    [SwaggerResponse(404, "Device not found or was removed.")]
    public async Task<IActionResult> GetDeviceById(int id, CancellationToken cancellationToken)
    {
        var query = new GetDeviceByIdQuery(id);
        var device = await deviceQueryService.Handle(query, cancellationToken);

        if (device == null) return NotFound("Device not found.");
        return Ok(DeviceResourceFromEntityAssembler.ToResourceFromEntity(device));
    }

    [HttpGet("owner/{ownerId:int}")]
    [SwaggerOperation("Get Devices by Owner Id", "List all active devices owned by a user.", OperationId = "GetDevicesByOwnerId")]
    [SwaggerResponse(200, "List of user devices retrieved.", typeof(IEnumerable<DeviceResource>))]
    public async Task<IActionResult> GetDevicesByOwnerId(int ownerId, CancellationToken cancellationToken)
    {
        var query = new GetDevicesByOwnerIdQuery(ownerId);
        var devices = await deviceQueryService.Handle(query, cancellationToken);
        var resources = devices.Select(DeviceResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet]
    [SwaggerOperation("Get All Devices", "List all registered active devices in the platform.", OperationId = "GetAllDevices")]
    [SwaggerResponse(200, "List of all active devices retrieved.", typeof(IEnumerable<DeviceResource>))]
    public async Task<IActionResult> GetAllDevices(CancellationToken cancellationToken)
    {
        var query = new GetAllDevicesQuery();
        var devices = await deviceQueryService.Handle(query, cancellationToken);
        var resources = devices.Select(DeviceResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation("Update Device Info", "Update name and room of an existing device.", OperationId = "UpdateDevice")]
    [SwaggerResponse(200, "Device info updated.", typeof(DeviceResource))]
    [SwaggerResponse(404, "Device not found.")]
    public async Task<IActionResult> UpdateDevice(int id, [FromBody] UpdateDeviceResource resource, CancellationToken cancellationToken)
    {
        var command = new UpdateDeviceCommand(id, resource.Name, resource.Room);
        var result = await deviceCommandService.Handle(command, cancellationToken);

        return DevicesActionResultAssembler.ToActionResult(
            this,
            result,
            problemDetailsFactory,
            device => Ok(DeviceResourceFromEntityAssembler.ToResourceFromEntity(device))
        );
    }

    [HttpPut("{id:int}/status")]
    [SwaggerOperation("Update Device Status", "Set the device status (ON, OFF, Maintenance).", OperationId = "UpdateDeviceStatus")]
    [SwaggerResponse(200, "Device status updated.", typeof(DeviceResource))]
    [SwaggerResponse(404, "Device not found.")]
    public async Task<IActionResult> UpdateDeviceStatus(int id, [FromBody] UpdateDeviceStatusResource resource, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<DeviceStatus>(resource.Status, true, out var status))
        {
            return BadRequest("Invalid status. Supported values are: ON, OFF, Maintenance.");
        }

        var command = new UpdateDeviceStatusCommand(id, status);
        var result = await deviceCommandService.Handle(command, cancellationToken);

        return DevicesActionResultAssembler.ToActionResult(
            this,
            result,
            problemDetailsFactory,
            device => Ok(DeviceResourceFromEntityAssembler.ToResourceFromEntity(device))
        );
    }

    [HttpPost("{id:int}/toggle")]
    [SwaggerOperation("Toggle Device", "Toggle device status between ON and OFF.", OperationId = "ToggleDevice")]
    [SwaggerResponse(200, "Device state toggled successfully.", typeof(DeviceResource))]
    [SwaggerResponse(404, "Device not found.")]
    public async Task<IActionResult> ToggleDevice(int id, CancellationToken cancellationToken)
    {
        var command = new ToggleDeviceCommand(id);
        var result = await deviceCommandService.Handle(command, cancellationToken);

        return DevicesActionResultAssembler.ToActionResult(
            this,
            result,
            problemDetailsFactory,
            device => Ok(DeviceResourceFromEntityAssembler.ToResourceFromEntity(device))
        );
    }

    [HttpDelete("{id:int}")]
    [SwaggerOperation("Delete Device", "Remove a device from the platform.", OperationId = "DeleteDevice")]
    [SwaggerResponse(200, "Device removed successfully.")]
    [SwaggerResponse(404, "Device not found.")]
    public async Task<IActionResult> DeleteDevice(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteDeviceCommand(id);
        var result = await deviceCommandService.Handle(command, cancellationToken);

        return DevicesActionResultAssembler.ToActionResult(
            this,
            result,
            problemDetailsFactory,
            () => Ok("Device removed successfully.")
        );
    }
}
