namespace ElectroCorp.Platform.Monitoring.Domain.Model.Queries;

public record GetEnergyStatisticsQuery(int DeviceId, string Period);
