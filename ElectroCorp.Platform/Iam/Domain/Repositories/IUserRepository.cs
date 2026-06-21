using ElectroCorp.Platform.Iam.Domain.Model.Aggregates;
using ElectroCorp.Platform.Shared.Domain.Repositories;

namespace ElectroCorp.Platform.Iam.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
}
