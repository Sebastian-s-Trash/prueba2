using ElectroCorp.Platform.Devices.Domain.Model.Aggregates;

namespace ElectroCorp.Platform.Devices.Domain.Model.Commands;

public record CreateDeviceCommand(string Name, string Type, int OwnerId, string Room);
public record UpdateDeviceCommand(int DeviceId, string Name, string Room);
public record UpdateDeviceStatusCommand(int DeviceId, DeviceStatus Status);
public record ToggleDeviceCommand(int DeviceId);
public record DeleteDeviceCommand(int DeviceId);
