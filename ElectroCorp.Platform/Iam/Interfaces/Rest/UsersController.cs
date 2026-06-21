using System.Net.Mime;
using ElectroCorp.Platform.Iam.Application.CommandServices;
using ElectroCorp.Platform.Iam.Application.QueryServices;
using ElectroCorp.Platform.Iam.Domain.Model.Commands;
using ElectroCorp.Platform.Iam.Domain.Model.Queries;
using ElectroCorp.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using ElectroCorp.Platform.Iam.Interfaces.Rest.Resources;
using ElectroCorp.Platform.Iam.Interfaces.Rest.Transform;
using ElectroCorp.Platform.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ElectroCorp.Platform.Iam.Interfaces.Rest;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available IAM Endpoints.")]
public class UsersController(
    IUserCommandService userCommandService,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost("sign-up")]
    [AllowAnonymous]
    [SwaggerOperation("User Registration", "Create a new user account.", OperationId = "SignUp")]
    [SwaggerResponse(201, "The user account was successfully created.")]
    [SwaggerResponse(400, "Invalid registration details or validation error.")]
    [SwaggerResponse(409, "Username or email is already taken.")]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource resource, CancellationToken cancellationToken)
    {
        var command = new SignUpCommand(resource.Username, resource.Email, resource.Password);
        var result = await userCommandService.Handle(command, cancellationToken);

        return IamActionResultAssembler.ToActionResult(
            this,
            result,
            problemDetailsFactory,
            () => StatusCode(StatusCodes.Status201Created, "User registered successfully.")
        );
    }

    [HttpPost("sign-in")]
    [AllowAnonymous]
    [SwaggerOperation("User Authentication", "Sign in with username and password.", OperationId = "SignIn")]
    [SwaggerResponse(200, "Authentication succeeded, returning user details and JWT token.", typeof(AuthenticatedUserResource))]
    [SwaggerResponse(401, "Invalid credentials.")]
    public async Task<IActionResult> SignIn([FromBody] SignInResource resource, CancellationToken cancellationToken)
    {
        var command = new SignInCommand(resource.Username, resource.Password);
        var result = await userCommandService.Handle(command, cancellationToken);

        return IamActionResultAssembler.ToActionResult(
            this,
            result,
            problemDetailsFactory,
            authInfo => Ok(UserResourceFromEntityAssembler.ToAuthenticatedResourceFromEntity(authInfo.user, authInfo.token))
        );
    }

    [HttpPut("{userId:int}")]
    [Authorize]
    [SwaggerOperation("Update User Profile", "Update profile information (username and email).", OperationId = "UpdateProfile")]
    [SwaggerResponse(200, "Profile updated successfully.", typeof(UserResource))]
    [SwaggerResponse(404, "User not found.")]
    [SwaggerResponse(409, "Database conflict or operation cancelled.")]
    public async Task<IActionResult> UpdateProfile(int userId, [FromBody] UpdateProfileResource resource, CancellationToken cancellationToken)
    {
        var command = new UpdateProfileCommand(userId, resource.Username, resource.Email);
        var result = await userCommandService.Handle(command, cancellationToken);

        return IamActionResultAssembler.ToActionResult(
            this,
            result,
            problemDetailsFactory,
            user => Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(user))
        );
    }

    [HttpPost("recover-password")]
    [AllowAnonymous]
    [SwaggerOperation("Recover Password", "Reset/recover password with new credentials.", OperationId = "RecoverPassword")]
    [SwaggerResponse(200, "Password updated successfully.")]
    [SwaggerResponse(404, "User not found.")]
    public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordResource resource, CancellationToken cancellationToken)
    {
        var command = new RecoverPasswordCommand(resource.Username, resource.NewPassword);
        var result = await userCommandService.Handle(command, cancellationToken);

        return IamActionResultAssembler.ToActionResult(
            this,
            result,
            problemDetailsFactory,
            () => Ok("Password updated successfully.")
        );
    }

    [HttpDelete("{userId:int}")]
    [Authorize]
    [SwaggerOperation("Delete User Account", "Deactivate/delete user account.", OperationId = "DeleteAccount")]
    [SwaggerResponse(200, "Account deleted successfully.")]
    [SwaggerResponse(404, "User not found.")]
    public async Task<IActionResult> DeleteAccount(int userId, CancellationToken cancellationToken)
    {
        var command = new DeleteAccountCommand(userId);
        var result = await userCommandService.Handle(command, cancellationToken);

        return IamActionResultAssembler.ToActionResult(
            this,
            result,
            problemDetailsFactory,
            () => Ok("Account deleted successfully.")
        );
    }
}
