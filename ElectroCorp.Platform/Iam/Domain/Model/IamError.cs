namespace ElectroCorp.Platform.Iam.Domain.Model;

public enum IamError
{
    UserNotFound,
    UsernameAlreadyTaken,
    EmailAlreadyRegistered,
    InvalidCredentials,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}
