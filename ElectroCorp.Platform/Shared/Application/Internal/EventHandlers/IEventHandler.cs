using ElectroCorp.Platform.Shared.Domain.Model.Events;
using Cortex.Mediator.Notifications;

namespace ElectroCorp.Platform.Shared.Application.Internal.EventHandlers;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IEvent
{
}
