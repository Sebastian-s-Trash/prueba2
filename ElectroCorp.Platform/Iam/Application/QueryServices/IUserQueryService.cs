using ElectroCorp.Platform.Iam.Domain.Model.Aggregates;
using ElectroCorp.Platform.Iam.Domain.Model.Queries;

namespace ElectroCorp.Platform.Iam.Application.QueryServices;

public interface IUserQueryService
{
    Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken = default);
    Task<User?> Handle(GetUserByUsernameQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken = default);
}
