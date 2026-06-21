using ElectroCorp.Platform.Billing.Domain.Model.Aggregates;
using ElectroCorp.Platform.Billing.Domain.Model.Commands;
using ElectroCorp.Platform.Shared.Application.Model;

namespace ElectroCorp.Platform.Billing.Application.CommandServices;

public interface ISubscriptionCommandService
{
    Task<Result<Subscription>> Handle(SubscribeCommand command, CancellationToken cancellationToken = default);
    Task<Result<Subscription>> Handle(CancelSubscriptionCommand command, CancellationToken cancellationToken = default);
    Task<Result<Subscription>> Handle(VerifySubscriptionCommand command, CancellationToken cancellationToken = default);
    Task<Result<Payment>> Handle(ProcessPaymentCommand command, CancellationToken cancellationToken = default);
}
