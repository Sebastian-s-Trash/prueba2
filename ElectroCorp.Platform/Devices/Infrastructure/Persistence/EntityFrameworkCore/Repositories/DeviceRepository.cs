using ElectroCorp.Platform.Devices.Domain.Model.Aggregates;
using ElectroCorp.Platform.Devices.Domain.Repositories;
using ElectroCorp.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using ElectroCorp.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ElectroCorp.Platform.Devices.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class DeviceRepository(AppDbContext context) : BaseRepository<Device>(context), IDeviceRepository
{
    public async Task<IEnumerable<Device>> FindByOwnerIdAsync(int ownerId, CancellationToken cancellationToken)
    {
        return await Context.Set<Device>()
            .Where(d => d.OwnerId == ownerId && d.Status != DeviceStatus.Removed)
            .ToListAsync(cancellationToken);
    }
}
