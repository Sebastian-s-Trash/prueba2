namespace ElectroCorp.Platform.Iam.Domain.Model.Commands;

public record UpdateProfileCommand(int UserId, string Username, string Email);
