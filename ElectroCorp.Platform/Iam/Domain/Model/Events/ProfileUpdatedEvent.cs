using ElectroCorp.Platform.Shared.Domain.Model.Events;

namespace ElectroCorp.Platform.Iam.Domain.Model.Events;

public record ProfileUpdatedEvent(int UserId, string Username, string Email) : IEvent;
