namespace ElectroCorp.Platform.Monitoring.Domain.Model;

public enum MonitoringError
{
    ReadingNotFound,
    DeviceNotFound,
    InvalidReadingValue,
    DatabaseError,
    OperationCancelled,
    InternalServerError
}
