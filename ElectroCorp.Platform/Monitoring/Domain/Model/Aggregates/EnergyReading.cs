using ElectroCorp.Platform.Shared.Domain.Model.Entities;

namespace ElectroCorp.Platform.Monitoring.Domain.Model.Aggregates;

public class EnergyReading : IAuditableEntity
{
    public EnergyReading()
    {
    }

    public EnergyReading(int deviceId, double consumptionValue)
    {
        DeviceId = deviceId;
        ConsumptionValue = consumptionValue;
        Timestamp = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public int DeviceId { get; private set; }
    public double ConsumptionValue { get; private set; }
    public DateTime Timestamp { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
