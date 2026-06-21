namespace ElectroCorp.Platform.Monitoring.Interfaces.Rest.Resources;

public record CreateEnergyReadingResource(int DeviceId, double ConsumptionValue);
public record EnergyReadingResource(int Id, int DeviceId, double ConsumptionValue, DateTime Timestamp);
public record EnergyStatisticsResource(int DeviceId, string Period, double TotalConsumption, double AverageConsumption, int ReadingsCount);
