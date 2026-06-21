using ElectroCorp.Platform.Devices.Domain.Model.Aggregates;
using ElectroCorp.Platform.Devices.Domain.Model.Queries;

namespace ElectroCorp.Platform.Devices.Application.QueryServices;

public interface IDeviceQueryService
{
    Task<Device?> Handle(GetDeviceByIdQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<Device>> Handle(GetDevicesByOwnerIdQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<Device>> Handle(GetAllDevicesQuery query, CancellationToken cancellationToken = default);
}
