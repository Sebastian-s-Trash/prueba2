using ElectroCorp.Platform.Devices.Application.CommandServices;
using ElectroCorp.Platform.Devices.Domain.Model;
using ElectroCorp.Platform.Devices.Domain.Model.Aggregates;
using ElectroCorp.Platform.Devices.Domain.Model.Commands;
using ElectroCorp.Platform.Devices.Domain.Model.Events;
using ElectroCorp.Platform.Devices.Domain.Repositories;
using ElectroCorp.Platform.Iam.Interfaces.Acl;
using ElectroCorp.Platform.Resources.Errors;
using ElectroCorp.Platform.Shared.Application.Model;
using ElectroCorp.Platform.Shared.Domain.Repositories;
using Cortex.Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace ElectroCorp.Platform.Devices.Application.Internal.CommandServices;

public class DeviceCommandService(
    IDeviceRepository deviceRepository,
    IIamContextFacade iamContextFacade,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    IStringLocalizer<ErrorMessages> localizer)
    : IDeviceCommandService
{
    private readonly IStringLocalizer<ErrorMessages> _localizer = localizer;

    public async Task<Result<Device>> Handle(CreateDeviceCommand command, CancellationToken cancellationToken)
    {
        // Verify owner exists using IAM ACL
        var username = await iamContextFacade.FetchUsernameByUserId(command.OwnerId, cancellationToken);
        if (string.IsNullOrEmpty(username))
            return Result<Device>.Failure(DeviceError.OwnerNotFound,
                _localizer[$"{nameof(DeviceError)}.{nameof(DeviceError.OwnerNotFound)}"]);

        if (string.IsNullOrWhiteSpace(command.Name) || string.IsNullOrWhiteSpace(command.Type))
            return Result<Device>.Failure(DeviceError.InvalidDeviceData,
                _localizer[$"{nameof(DeviceError)}.{nameof(DeviceError.InvalidDeviceData)}"]);

        var device = new Device(command.Name, command.Type, command.OwnerId, command.Room);
        try
        {
            await deviceRepository.AddAsync(device, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            await mediator.PublishAsync(new DeviceCreatedEvent(device.Id, device.Name, device.Type, device.OwnerId), cancellationToken);

            return Result<Device>.Success(device);
        }
        catch (Exception)
        {
            return Result<Device>.Failure(DeviceError.DatabaseError, _localizer[$"{nameof(DeviceError)}.{nameof(DeviceError.DatabaseError)}"]);
        }
    }

    public async Task<Result<Device>> Handle(UpdateDeviceCommand command, CancellationToken cancellationToken)
    {
        var device = await deviceRepository.FindByIdAsync(command.DeviceId, cancellationToken);
        if (device == null || device.Status == DeviceStatus.Removed)
            return Result<Device>.Failure(DeviceError.DeviceNotFound,
                _localizer[$"{nameof(DeviceError)}.{nameof(DeviceError.DeviceNotFound)}"]);

        device.UpdateInfo(command.Name, command.Room);
        try
        {
            deviceRepository.Update(device);
            await unitOfWork.CompleteAsync(cancellationToken);

            await mediator.PublishAsync(new DeviceUpdatedEvent(device.Id, device.Name, device.Room), cancellationToken);

            return Result<Device>.Success(device);
        }
        catch (Exception)
        {
            return Result<Device>.Failure(DeviceError.DatabaseError, _localizer[$"{nameof(DeviceError)}.{nameof(DeviceError.DatabaseError)}"]);
        }
    }

    public async Task<Result<Device>> Handle(UpdateDeviceStatusCommand command, CancellationToken cancellationToken)
    {
        var device = await deviceRepository.FindByIdAsync(command.DeviceId, cancellationToken);
        if (device == null || device.Status == DeviceStatus.Removed)
            return Result<Device>.Failure(DeviceError.DeviceNotFound,
                _localizer[$"{nameof(DeviceError)}.{nameof(DeviceError.DeviceNotFound)}"]);

        device.UpdateStatus(command.Status);
        try
        {
            deviceRepository.Update(device);
            await unitOfWork.CompleteAsync(cancellationToken);

            await mediator.PublishAsync(new DeviceStatusUpdatedEvent(device.Id, device.Status.ToString()), cancellationToken);

            return Result<Device>.Success(device);
        }
        catch (Exception)
        {
            return Result<Device>.Failure(DeviceError.DatabaseError, _localizer[$"{nameof(DeviceError)}.{nameof(DeviceError.DatabaseError)}"]);
        }
    }

    public async Task<Result<Device>> Handle(ToggleDeviceCommand command, CancellationToken cancellationToken)
    {
        var device = await deviceRepository.FindByIdAsync(command.DeviceId, cancellationToken);
        if (device == null || device.Status == DeviceStatus.Removed)
            return Result<Device>.Failure(DeviceError.DeviceNotFound,
                _localizer[$"{nameof(DeviceError)}.{nameof(DeviceError.DeviceNotFound)}"]);

        device.Toggle();
        try
        {
            deviceRepository.Update(device);
            await unitOfWork.CompleteAsync(cancellationToken);

            await mediator.PublishAsync(new DeviceToggledEvent(device.Id, device.Status.ToString()), cancellationToken);

            return Result<Device>.Success(device);
        }
        catch (Exception)
        {
            return Result<Device>.Failure(DeviceError.DatabaseError, _localizer[$"{nameof(DeviceError)}.{nameof(DeviceError.DatabaseError)}"]);
        }
    }

    public async Task<Result> Handle(DeleteDeviceCommand command, CancellationToken cancellationToken)
    {
        var device = await deviceRepository.FindByIdAsync(command.DeviceId, cancellationToken);
        if (device == null || device.Status == DeviceStatus.Removed)
            return Result.Failure(DeviceError.DeviceNotFound,
                _localizer[$"{nameof(DeviceError)}.{nameof(DeviceError.DeviceNotFound)}"]);

        device.MarkAsRemoved();
        try
        {
            deviceRepository.Update(device);
            await unitOfWork.CompleteAsync(cancellationToken);

            await mediator.PublishAsync(new DeviceDeletedEvent(device.Id, device.OwnerId, device.Name), cancellationToken);

            return Result.Success();
        }
        catch (Exception)
        {
            return Result.Failure(DeviceError.DatabaseError, _localizer[$"{nameof(DeviceError)}.{nameof(DeviceError.DatabaseError)}"]);
        }
    }
}
