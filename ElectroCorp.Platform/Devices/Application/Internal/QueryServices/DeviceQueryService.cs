using ElectroCorp.Platform.Devices.Application.QueryServices;
using ElectroCorp.Platform.Devices.Domain.Model.Aggregates;
using ElectroCorp.Platform.Devices.Domain.Model.Queries;
using ElectroCorp.Platform.Devices.Domain.Repositories;

namespace ElectroCorp.Platform.Devices.Application.Internal.QueryServices;

public class DeviceQueryService(IDeviceRepository deviceRepository) : IDeviceQueryService
{
    public async Task<Device?> Handle(GetDeviceByIdQuery query, CancellationToken cancellationToken)
    {
        var device = await deviceRepository.FindByIdAsync(query.DeviceId, cancellationToken);
        if (device != null && device.Status == DeviceStatus.Removed) return null;
        return device;
    }

    public async Task<IEnumerable<Device>> Handle(GetDevicesByOwnerIdQuery query, CancellationToken cancellationToken)
    {
        return await deviceRepository.FindByOwnerIdAsync(query.OwnerId, cancellationToken);
    }

    public async Task<IEnumerable<Device>> Handle(GetAllDevicesQuery query, CancellationToken cancellationToken)
    {
        var list = await deviceRepository.ListAsync(cancellationToken);
        return list.Where(d => d.Status != DeviceStatus.Removed);
    }
}
