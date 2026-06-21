using ElectroCorp.Platform.Shared.Domain.Model.Entities;

namespace ElectroCorp.Platform.Notifications.Domain.Model.Aggregates;

public class Alert : IAuditableEntity
{
    public Alert()
    {
        Message = string.Empty;
        Type = "Info";
    }

    public Alert(int userId, string message, string type)
    {
        UserId = userId;
        Message = message;
        Type = type;
        IsRead = false;
        Timestamp = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Message { get; private set; }
    public string Type { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime Timestamp { get; private set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public Alert MarkAsRead()
    {
        IsRead = true;
        return this;
    }
}
