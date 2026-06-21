using ElectroCorp.Platform.Shared.Domain.Model.Events;

namespace ElectroCorp.Platform.Iam.Domain.Model.Events;

public record PasswordRecoveredEvent(int UserId) : IEvent;
