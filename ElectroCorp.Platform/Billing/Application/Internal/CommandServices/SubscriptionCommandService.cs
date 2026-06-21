using ElectroCorp.Platform.Billing.Application.CommandServices;
using ElectroCorp.Platform.Billing.Domain.Model;
using ElectroCorp.Platform.Billing.Domain.Model.Aggregates;
using ElectroCorp.Platform.Billing.Domain.Model.Commands;
using ElectroCorp.Platform.Billing.Domain.Model.Events;
using ElectroCorp.Platform.Billing.Domain.Repositories;
using ElectroCorp.Platform.Resources.Errors;
using ElectroCorp.Platform.Shared.Application.Model;
using ElectroCorp.Platform.Shared.Domain.Repositories;
using Cortex.Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace ElectroCorp.Platform.Billing.Application.Internal.CommandServices;

public class SubscriptionCommandService(
    ISubscriptionRepository subscriptionRepository,
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    IStringLocalizer<ErrorMessages> localizer)
    : ISubscriptionCommandService
{
    private readonly IStringLocalizer<ErrorMessages> _localizer = localizer;

    public async Task<Result<Subscription>> Handle(SubscribeCommand command, CancellationToken cancellationToken)
    {
        var existing = await subscriptionRepository.FindByUserIdAsync(command.UserId, cancellationToken);
        if (existing != null && existing.Status == SubscriptionStatus.Active)
            return Result<Subscription>.Failure(BillingError.SubscriptionAlreadyActive,
                _localizer[$"{nameof(BillingError)}.{nameof(BillingError.SubscriptionAlreadyActive)}"]);

        // Validate plan
        if (string.IsNullOrWhiteSpace(command.PlanName))
            return Result<Subscription>.Failure(BillingError.InvalidPlan,
                _localizer[$"{nameof(BillingError)}.{nameof(BillingError.InvalidPlan)}"]);

        Subscription subscription;
        if (existing != null)
        {
            // Update existing expired/canceled subscription to new active one
            subscription = new Subscription(command.UserId, command.PlanName);
            subscriptionRepository.Remove(existing);
            await subscriptionRepository.AddAsync(subscription, cancellationToken);
        }
        else
        {
            subscription = new Subscription(command.UserId, command.PlanName);
            await subscriptionRepository.AddAsync(subscription, cancellationToken);
        }

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
            await mediator.PublishAsync(new SubscriptionActivatedEvent(subscription.Id, subscription.UserId, subscription.PlanName), cancellationToken);
            return Result<Subscription>.Success(subscription);
        }
        catch (Exception)
        {
            return Result<Subscription>.Failure(BillingError.DatabaseError, _localizer[$"{nameof(BillingError)}.{nameof(BillingError.DatabaseError)}"]);
        }
    }

    public async Task<Result<Subscription>> Handle(CancelSubscriptionCommand command, CancellationToken cancellationToken)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(command.SubscriptionId, cancellationToken);
        if (subscription == null)
            return Result<Subscription>.Failure(BillingError.SubscriptionNotFound,
                _localizer[$"{nameof(BillingError)}.{nameof(BillingError.SubscriptionNotFound)}"]);

        subscription.Cancel();
        try
        {
            subscriptionRepository.Update(subscription);
            await unitOfWork.CompleteAsync(cancellationToken);
            await mediator.PublishAsync(new SubscriptionCanceledEvent(subscription.Id), cancellationToken);
            return Result<Subscription>.Success(subscription);
        }
        catch (Exception)
        {
            return Result<Subscription>.Failure(BillingError.DatabaseError, _localizer[$"{nameof(BillingError)}.{nameof(BillingError.DatabaseError)}"]);
        }
    }

    public async Task<Result<Subscription>> Handle(VerifySubscriptionCommand command, CancellationToken cancellationToken)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(command.SubscriptionId, cancellationToken);
        if (subscription == null)
            return Result<Subscription>.Failure(BillingError.SubscriptionNotFound,
                _localizer[$"{nameof(BillingError)}.{nameof(BillingError.SubscriptionNotFound)}"]);

        subscription.VerifyStatus();
        try
        {
            subscriptionRepository.Update(subscription);
            await unitOfWork.CompleteAsync(cancellationToken);
            await mediator.PublishAsync(new SubscriptionVerifiedEvent(subscription.Id, subscription.Status.ToString()), cancellationToken);
            return Result<Subscription>.Success(subscription);
        }
        catch (Exception)
        {
            return Result<Subscription>.Failure(BillingError.DatabaseError, _localizer[$"{nameof(BillingError)}.{nameof(BillingError.DatabaseError)}"]);
        }
    }

    public async Task<Result<Payment>> Handle(ProcessPaymentCommand command, CancellationToken cancellationToken)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(command.SubscriptionId, cancellationToken);
        if (subscription == null)
            return Result<Payment>.Failure(BillingError.SubscriptionNotFound,
                _localizer[$"{nameof(BillingError)}.{nameof(BillingError.SubscriptionNotFound)}"]);

        // Simulate payment gateway (always succeeds unless amount is negative)
        var status = command.Amount > 0 ? PaymentStatus.Success : PaymentStatus.Failed;
        var payment = new Payment(command.SubscriptionId, command.Amount, status);

        try
        {
            await paymentRepository.AddAsync(payment, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            if (status == PaymentStatus.Success)
            {
                await mediator.PublishAsync(new PaymentRegisteredEvent(payment.Id, payment.SubscriptionId, payment.Amount), cancellationToken);
            }

            return Result<Payment>.Success(payment);
        }
        catch (Exception)
        {
            return Result<Payment>.Failure(BillingError.DatabaseError, _localizer[$"{nameof(BillingError)}.{nameof(BillingError.DatabaseError)}"]);
        }
    }
}
