namespace ElectroCorp.Platform.Devices.Domain.Model.Commands;

public record CreateDeviceCommand(string Name, string Type, int OwnerId, string Room);
