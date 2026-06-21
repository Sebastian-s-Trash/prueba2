namespace ElectroCorp.Platform.Iam.Interfaces.Rest.Resources;

public record SignUpResource(string Username, string Email, string Password);
public record SignInResource(string Username, string Password);
public record UserResource(int Id, string Username, string Email);
public record AuthenticatedUserResource(int Id, string Username, string Email, string Token);
public record UpdateProfileResource(string Username, string Email);
public record RecoverPasswordResource(string Username, string NewPassword);
