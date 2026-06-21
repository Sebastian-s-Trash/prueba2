using ElectroCorp.Platform.Iam.Domain.Model.Aggregates;
using ElectroCorp.Platform.Iam.Domain.Model.Commands;
using ElectroCorp.Platform.Shared.Application.Model;

namespace ElectroCorp.Platform.Iam.Application.CommandServices;

public interface IUserCommandService
{
    Task<Result<(User user, string token)>> Handle(SignInCommand command, CancellationToken cancellationToken = default);
    Task<Result> Handle(SignUpCommand command, CancellationToken cancellationToken = default);
    Task<Result<User>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken = default);
    Task<Result> Handle(RecoverPasswordCommand command, CancellationToken cancellationToken = default);
    Task<Result> Handle(DeleteAccountCommand command, CancellationToken cancellationToken = default);
}
