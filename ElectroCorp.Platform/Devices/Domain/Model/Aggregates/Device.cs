using ElectroCorp.Platform.Shared.Domain.Model.Entities;

namespace ElectroCorp.Platform.Devices.Domain.Model.Aggregates;


public class Device : IAuditableEntity
{
    public Device()
    {
        Name = string.Empty;
        Type = string.Empty;
        Status = DeviceStatus.OFF;
        Room = string.Empty;
    }

    public Device(string name, string type, int ownerId, string room)
    {
        Name = name;
        Type = type;
        OwnerId = ownerId;
        Room = room;
        Status = DeviceStatus.OFF;
        DateAdded = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Type { get; private set; }
    public DeviceStatus Status { get; private set; }
    public int OwnerId { get; private set; }
    public string Room { get; private set; }
    public DateTime DateAdded { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public Device UpdateInfo(string name, string room)
    {
        Name = name;
        Room = room;
        return this;
    }

    public Device UpdateStatus(DeviceStatus status)
    {
        Status = status;
        return this;
    }

    public Device Toggle()
    {
        Status = Status == DeviceStatus.ON ? DeviceStatus.OFF : DeviceStatus.ON;
        return this;
    }

    public Device MarkAsRemoved()
    {
        Status = DeviceStatus.Removed;
        return this;
    }
}
