namespace ElectroCorp.Platform.Notifications.Interfaces.Rest.Resources;

public record CreateAlertResource(int UserId, string Message, string Type);
public record AlertResource(int Id, int UserId, string Message, string Type, bool IsRead, DateTime Timestamp);
