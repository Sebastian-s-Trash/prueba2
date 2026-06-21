namespace ElectroCorp.Platform.Iam.Domain.Model.Commands;

public record SignUpCommand(string Username, string Email, string Password);
public record SignInCommand(string Username, string Password);
public record UpdateProfileCommand(int UserId, string Username, string Email);
public record RecoverPasswordCommand(string Username, string NewPassword);
public record DeleteAccountCommand(int UserId);
