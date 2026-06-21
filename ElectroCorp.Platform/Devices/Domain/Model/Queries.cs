namespace ElectroCorp.Platform.Devices.Domain.Model.Queries;

public record GetDeviceByIdQuery(int DeviceId);
public record GetDevicesByOwnerIdQuery(int OwnerId);
public record GetAllDevicesQuery;
