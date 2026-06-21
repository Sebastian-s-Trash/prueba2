using ElectroCorp.Platform.Shared.Domain.Model.Events;

namespace ElectroCorp.Platform.Iam.Domain.Model.Events;

public record UserRegisteredEvent(int UserId, string Username) : IEvent;
public record ProfileUpdatedEvent(int UserId, string Username, string Email) : IEvent;
public record PasswordRecoveredEvent(int UserId) : IEvent;
public record AccountDeletedEvent(int UserId) : IEvent;
