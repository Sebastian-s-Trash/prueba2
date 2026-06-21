using ElectroCorp.Platform.Iam.Application.Internal.OutboundServices;
using ElectroCorp.Platform.Iam.Application.QueryServices;
using ElectroCorp.Platform.Iam.Domain.Model.Queries;
using ElectroCorp.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;

namespace ElectroCorp.Platform.Iam.Infrastructure.Pipeline.Middleware.Components;

public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IUserQueryService userQueryService,
        ITokenService tokenService)
    {
        Console.WriteLine($"\n[Auth] ---> Petición entrante a: {context.Request.Path}");
        var endpoint = context.GetEndpoint();

        if (endpoint == null)
        {
            Console.WriteLine("[Auth] FALLO: Endpoint es nulo. Pasando al siguiente middleware...");
            await next(context);
            return;
        }

        var allowAnonymous = endpoint.Metadata
            .Any(m => m.GetType() == typeof(AllowAnonymousAttribute));

        if (allowAnonymous)
        {
            Console.WriteLine("[Auth] ÉXITO: Endpoint público (AllowAnonymous).");
            await next(context);
            return;
        }

        var token = context.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last();
        Console.WriteLine($"[Auth] Header Authorization recibido: {(string.IsNullOrEmpty(token) ? "NINGUNO" : "SÍ LLEGÓ")}");
        if (token == null) 
        {
            Console.WriteLine("[Auth] FALLO: Token nulo o formato inválido.");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: Missing or invalid token format.");
            return;
        }
        Console.WriteLine("[Auth] Validando firma del token...");
        var userId = await tokenService.ValidateToken(token);

        if (userId == null) 
        {
            Console.WriteLine("[Auth] FALLO: La validación interna del token falló (TokenService retornó null).");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: Token validation failed.");
            return;
        }
        Console.WriteLine($"[Auth] Token válido. Buscando al usuario ID: {userId}");
        var getUserByIdQuery = new GetUserByIdQuery(userId.Value);
        var user = await userQueryService.Handle(getUserByIdQuery, context.RequestAborted);

        if (user == null || user.IsDeleted)
        {
            Console.WriteLine("[Auth] FALLO: Usuario no encontrado en la base de datos o está eliminado.");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: User not found or deleted.");
            return;
        }
        Console.WriteLine("[Auth] ¡ÉXITO TOTAL! Usuario autorizado. Guardando en contexto.");
        context.Items["User"] = user;

        await next(context);
    }
}
