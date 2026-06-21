using ElectroCorp.Platform.Shared.Domain.Repositories;
using ElectroCorp.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace ElectroCorp.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Unit of work for the application.
/// </summary>
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
