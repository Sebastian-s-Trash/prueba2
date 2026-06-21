namespace ElectroCorp.Platform.Devices.Interfaces.Rest.Resources;

public record CreateDeviceResource(string Name, string Type, int OwnerId, string Room);
public record UpdateDeviceResource(string Name, string Room);
public record UpdateDeviceStatusResource(string Status);
public record DeviceResource(int Id, string Name, string Type, string Status, int OwnerId, string Room, DateTime DateAdded);
