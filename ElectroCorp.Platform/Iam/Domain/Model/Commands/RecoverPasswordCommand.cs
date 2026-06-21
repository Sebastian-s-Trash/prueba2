namespace ElectroCorp.Platform.Iam.Domain.Model.Commands;

public record RecoverPasswordCommand(string Username, string NewPassword);
