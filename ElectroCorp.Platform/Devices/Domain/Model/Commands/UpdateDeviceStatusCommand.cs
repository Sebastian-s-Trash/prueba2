using ElectroCorp.Platform.Devices.Domain.Model.Aggregates;

namespace ElectroCorp.Platform.Devices.Domain.Model.Commands;

public record UpdateDeviceStatusCommand(int DeviceId, DeviceStatus Status);
