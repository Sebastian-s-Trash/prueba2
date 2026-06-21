namespace ElectroCorp.Platform.Monitoring.Domain.Model.Queries;

public record GetReadingsByDeviceIdQuery(int DeviceId);
public record GetEnergyStatisticsQuery(int DeviceId, string Period);
