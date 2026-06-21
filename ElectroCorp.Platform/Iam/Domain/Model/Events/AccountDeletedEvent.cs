using ElectroCorp.Platform.Shared.Domain.Model.Events;

namespace ElectroCorp.Platform.Iam.Domain.Model.Events;

public record AccountDeletedEvent(int UserId) : IEvent;
