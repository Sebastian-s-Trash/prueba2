namespace ElectroCorp.Platform.Notifications.Domain.Model.Commands;

public record CreateAlertCommand(int UserId, string Message, string Type);
public record MarkAlertAsReadCommand(int AlertId);
