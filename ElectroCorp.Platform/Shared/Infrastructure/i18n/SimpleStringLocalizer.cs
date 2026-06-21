using Microsoft.Extensions.Localization;

namespace ElectroCorp.Platform.Shared.Infrastructure.i18n;

public class SimpleStringLocalizer<T> : IStringLocalizer<T>
{
    private static readonly Dictionary<string, string> Dictionary = new()
    {
        // General
        { "GenericError", "An error occurred." },
        { "OperationCancelled", "The operation was cancelled." },
        { "InternalServerError", "Internal server error." },

        // IAM
        { "IamError.UserNotFound", "User not found." },
        { "IamError.UsernameAlreadyTaken", "Username '{0}' is already taken." },
        { "IamError.EmailAlreadyRegistered", "Email address '{0}' is already registered." },
        { "IamError.InvalidCredentials", "Invalid username or password." },
        { "IamError.OperationCancelled", "User operation was cancelled." },
        { "IamError.DatabaseError", "A database error occurred during IAM operation." },
        { "IamError.InternalServerError", "An unexpected internal error occurred in IAM." },

        // Billing
        { "BillingError.SubscriptionNotFound", "Subscription not found." },
        { "BillingError.PaymentNotFound", "Payment not found." },
        { "BillingError.SubscriptionAlreadyActive", "User already has an active subscription." },
        { "BillingError.InvalidPlan", "Invalid plan name provided." },
        { "BillingError.DatabaseError", "A database error occurred during billing." },
        { "BillingError.InternalServerError", "An unexpected error occurred in Billing." },

        // Devices
        { "DeviceError.DeviceNotFound", "Device not found." },
        { "DeviceError.OwnerNotFound", "Owner user not found." },
        { "DeviceError.InvalidDeviceData", "Invalid device data provided." },
        { "DeviceError.DeviceAlreadyRemoved", "Device was already removed." },
        { "DeviceError.DatabaseError", "A database error occurred in Devices." },
        { "DeviceError.InternalServerError", "An unexpected internal error occurred in Devices." },

        // Monitoring
        { "MonitoringError.ReadingNotFound", "Energy reading not found." },
        { "MonitoringError.DeviceNotFound", "Associated device not found." },
        { "MonitoringError.InvalidReadingValue", "Invalid energy consumption reading value (must be >= 0)." },
        { "MonitoringError.DatabaseError", "A database error occurred in Monitoring." },
        { "MonitoringError.InternalServerError", "An unexpected internal error occurred in Monitoring." },

        // Notifications
        { "NotificationError.AlertNotFound", "Alert not found." },
        { "NotificationError.UserNotFound", "Target user not found." },
        { "NotificationError.DatabaseError", "A database error occurred in Notifications." },
        { "NotificationError.InternalServerError", "An unexpected internal error occurred in Notifications." }
    };

    public LocalizedString this[string name]
    {
        get
        {
            var value = Dictionary.GetValueOrDefault(name, name);
            return new LocalizedString(name, value, resourceNotFound: !Dictionary.ContainsKey(name));
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var format = Dictionary.GetValueOrDefault(name, name);
            try
            {
                var value = string.Format(format, arguments);
                return new LocalizedString(name, value, resourceNotFound: !Dictionary.ContainsKey(name));
            }
            catch (FormatException)
            {
                return new LocalizedString(name, format, resourceNotFound: !Dictionary.ContainsKey(name));
            }
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        foreach (var (key, value) in Dictionary)
        {
            yield return new LocalizedString(key, value);
        }
    }
}
