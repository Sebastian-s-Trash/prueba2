using ElectroCorp.Platform.Devices.Domain.Model.Aggregates;
using ElectroCorp.Platform.Devices.Domain.Model.Commands;
using ElectroCorp.Platform.Shared.Application.Model;

namespace ElectroCorp.Platform.Devices.Application.CommandServices;

public interface IDeviceCommandService
{
    Task<Result<Device>> Handle(CreateDeviceCommand command, CancellationToken cancellationToken = default);
    Task<Result<Device>> Handle(UpdateDeviceCommand command, CancellationToken cancellationToken = default);
    Task<Result<Device>> Handle(UpdateDeviceStatusCommand command, CancellationToken cancellationToken = default);
    Task<Result<Device>> Handle(ToggleDeviceCommand command, CancellationToken cancellationToken = default);
    Task<Result> Handle(DeleteDeviceCommand command, CancellationToken cancellationToken = default);
}
