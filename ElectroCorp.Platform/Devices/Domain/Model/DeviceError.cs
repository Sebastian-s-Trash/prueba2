namespace ElectroCorp.Platform.Devices.Domain.Model;

public enum DeviceError
{
    DeviceNotFound,
    OwnerNotFound,
    InvalidDeviceData,
    DeviceAlreadyRemoved,
    DatabaseError,
    OperationCancelled,
    InternalServerError
}
