using ElectroCorp.Platform.Monitoring.Application.CommandServices;
using ElectroCorp.Platform.Monitoring.Domain.Model;
using ElectroCorp.Platform.Monitoring.Domain.Model.Aggregates;
using ElectroCorp.Platform.Monitoring.Domain.Model.Commands;
using ElectroCorp.Platform.Monitoring.Domain.Model.Events;
using ElectroCorp.Platform.Monitoring.Domain.Repositories;
using ElectroCorp.Platform.Devices.Application.QueryServices;
using ElectroCorp.Platform.Devices.Domain.Model.Queries;
using ElectroCorp.Platform.Resources.Errors;
using ElectroCorp.Platform.Shared.Application.Model;
using ElectroCorp.Platform.Shared.Domain.Repositories;
using Cortex.Mediator;
using Microsoft.Extensions.Localization;

namespace ElectroCorp.Platform.Monitoring.Application.Internal.CommandServices;

public class EnergyReadingCommandService(
    IEnergyReadingRepository energyReadingRepository,
    IDeviceQueryService deviceQueryService,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    IStringLocalizer<ErrorMessages> localizer)
    : IEnergyReadingCommandService
{
    private readonly IStringLocalizer<ErrorMessages> _localizer = localizer;

    public async Task<Result<EnergyReading>> Handle(CreateEnergyReadingCommand command, CancellationToken cancellationToken)
    {
        // Verify device exists using Devices Context
        var getDeviceQuery = new GetDeviceByIdQuery(command.DeviceId);
        var device = await deviceQueryService.Handle(getDeviceQuery, cancellationToken);
        if (device == null)
            return Result<EnergyReading>.Failure(MonitoringError.DeviceNotFound,
                _localizer[$"{nameof(MonitoringError)}.{nameof(MonitoringError.DeviceNotFound)}"]);

        if (command.ConsumptionValue < 0)
            return Result<EnergyReading>.Failure(MonitoringError.InvalidReadingValue,
                _localizer[$"{nameof(MonitoringError)}.{nameof(MonitoringError.InvalidReadingValue)}"]);

        var reading = new EnergyReading(command.DeviceId, command.ConsumptionValue);
        try
        {
            await energyReadingRepository.AddAsync(reading, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            await mediator.PublishAsync(new EnergyReadingCreatedEvent(reading.Id, reading.DeviceId, reading.ConsumptionValue), cancellationToken);

            return Result<EnergyReading>.Success(reading);
        }
        catch (Exception)
        {
            return Result<EnergyReading>.Failure(MonitoringError.DatabaseError, _localizer[$"{nameof(MonitoringError)}.{nameof(MonitoringError.DatabaseError)}"]);
        }
    }
}
