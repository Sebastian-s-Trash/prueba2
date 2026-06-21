using ElectroCorp.Platform.Iam.Application.CommandServices;
using ElectroCorp.Platform.Iam.Application.Internal.OutboundServices;
using ElectroCorp.Platform.Iam.Domain.Model;
using ElectroCorp.Platform.Iam.Domain.Model.Aggregates;
using ElectroCorp.Platform.Iam.Domain.Model.Commands;
using ElectroCorp.Platform.Iam.Domain.Model.Events;
using ElectroCorp.Platform.Iam.Domain.Repositories;
using ElectroCorp.Platform.Resources.Errors;
using ElectroCorp.Platform.Shared.Application.Model;
using ElectroCorp.Platform.Shared.Domain.Repositories;
using Cortex.Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace ElectroCorp.Platform.Iam.Application.Internal.CommandServices;

public class UserCommandService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IHashingService hashingService,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    IStringLocalizer<ErrorMessages> localizer)
    : IUserCommandService
{
    private readonly IStringLocalizer<ErrorMessages> _localizer = localizer;

    public async Task<Result<(User user, string token)>> Handle(SignInCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByUsernameAsync(command.Username, cancellationToken);

        if (user == null || !hashingService.VerifyPassword(command.Password, user.PasswordHash))
            return Result<(User user, string token)>.Failure(IamError.InvalidCredentials,
                _localizer[$"{nameof(IamError)}.{nameof(IamError.InvalidCredentials)}"]);

        var token = tokenService.GenerateToken(user);

        return Result<(User user, string token)>.Success((user, token));
    }

    public async Task<Result> Handle(SignUpCommand command, CancellationToken cancellationToken)
    {
        if (await userRepository.ExistsByUsernameAsync(command.Username, cancellationToken))
            return Result.Failure(IamError.UsernameAlreadyTaken,
                _localizer[$"{nameof(IamError)}.{nameof(IamError.UsernameAlreadyTaken)}", command.Username]);

        if (await userRepository.ExistsByEmailAsync(command.Email, cancellationToken))
            return Result.Failure(IamError.EmailAlreadyRegistered,
                _localizer[$"{nameof(IamError)}.{nameof(IamError.EmailAlreadyRegistered)}", command.Email]);

        var hashedPassword = hashingService.HashPassword(command.Password);
        var user = new User(command.Username, command.Email, hashedPassword);
        try
        {
            await userRepository.AddAsync(user, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            
            // Publish UserRegistered event
            await mediator.PublishAsync(new UserRegisteredEvent(user.Id, user.Username), cancellationToken);

            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(IamError.OperationCancelled, _localizer[$"{nameof(IamError)}.{nameof(IamError.OperationCancelled)}"]);
        }
        catch (DbUpdateException)
        {
            return Result.Failure(IamError.DatabaseError, _localizer[$"{nameof(IamError)}.{nameof(IamError.DatabaseError)}"]);
        }
        catch (Exception)
        {
            return Result.Failure(IamError.InternalServerError, _localizer[$"{nameof(IamError)}.{nameof(IamError.InternalServerError)}"]);
        }
    }

    public async Task<Result<User>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByIdAsync(command.UserId, cancellationToken);
        if (user == null || user.IsDeleted)
            return Result<User>.Failure(IamError.UserNotFound, _localizer[$"{nameof(IamError)}.{nameof(IamError.UserNotFound)}"]);

        user.UpdateProfile(command.Username, command.Email);
        try
        {
            userRepository.Update(user);
            await unitOfWork.CompleteAsync(cancellationToken);

            // Publish ProfileUpdated event
            await mediator.PublishAsync(new ProfileUpdatedEvent(user.Id, user.Username, user.Email), cancellationToken);

            return Result<User>.Success(user);
        }
        catch (DbUpdateException)
        {
            return Result<User>.Failure(IamError.DatabaseError, _localizer[$"{nameof(IamError)}.{nameof(IamError.DatabaseError)}"]);
        }
        catch (Exception)
        {
            return Result<User>.Failure(IamError.InternalServerError, _localizer[$"{nameof(IamError)}.{nameof(IamError.InternalServerError)}"]);
        }
    }

    public async Task<Result> Handle(RecoverPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByUsernameAsync(command.Username, cancellationToken);
        if (user == null)
            return Result.Failure(IamError.UserNotFound, _localizer[$"{nameof(IamError)}.{nameof(IamError.UserNotFound)}"]);

        var hashedPassword = hashingService.HashPassword(command.NewPassword);
        user.UpdatePasswordHash(hashedPassword);
        try
        {
            userRepository.Update(user);
            await unitOfWork.CompleteAsync(cancellationToken);

            // Publish PasswordRecovered event
            await mediator.PublishAsync(new PasswordRecoveredEvent(user.Id), cancellationToken);

            return Result.Success();
        }
        catch (DbUpdateException)
        {
            return Result.Failure(IamError.DatabaseError, _localizer[$"{nameof(IamError)}.{nameof(IamError.DatabaseError)}"]);
        }
        catch (Exception)
        {
            return Result.Failure(IamError.InternalServerError, _localizer[$"{nameof(IamError)}.{nameof(IamError.InternalServerError)}"]);
        }
    }

    public async Task<Result> Handle(DeleteAccountCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByIdAsync(command.UserId, cancellationToken);
        if (user == null || user.IsDeleted)
            return Result.Failure(IamError.UserNotFound, _localizer[$"{nameof(IamError)}.{nameof(IamError.UserNotFound)}"]);

        user.MarkAsDeleted();
        try
        {
            userRepository.Update(user);
            await unitOfWork.CompleteAsync(cancellationToken);

            // Publish AccountDeleted event
            await mediator.PublishAsync(new AccountDeletedEvent(user.Id), cancellationToken);

            return Result.Success();
        }
        catch (DbUpdateException)
        {
            return Result.Failure(IamError.DatabaseError, _localizer[$"{nameof(IamError)}.{nameof(IamError.DatabaseError)}"]);
        }
        catch (Exception)
        {
            return Result.Failure(IamError.InternalServerError, _localizer[$"{nameof(IamError)}.{nameof(IamError.InternalServerError)}"]);
        }
    }
}
