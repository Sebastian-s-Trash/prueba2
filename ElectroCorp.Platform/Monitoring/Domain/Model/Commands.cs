namespace ElectroCorp.Platform.Monitoring.Domain.Model.Commands;

public record CreateEnergyReadingCommand(int DeviceId, double ConsumptionValue);
