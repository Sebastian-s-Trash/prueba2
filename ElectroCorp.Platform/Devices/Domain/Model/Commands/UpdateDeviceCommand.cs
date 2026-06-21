namespace ElectroCorp.Platform.Devices.Domain.Model.Commands;

public record UpdateDeviceCommand(int DeviceId, string Name, string Room);
