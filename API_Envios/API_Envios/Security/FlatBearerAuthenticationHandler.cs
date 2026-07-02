using System.Text.Encodings.Web;
using API_Envios.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace API_Envios.Security;

public class FlatBearerAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly TokenService _tokenService;

    public FlatBearerAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        TokenService tokenService)
        : base(options, logger, encoder)
    {
        _tokenService = tokenService;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorization = Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(authorization))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.Fail("Formato de autorizacion invalido."));
        }

        var token = authorization["Bearer ".Length..].Trim();
        var principal = _tokenService.ValidateToken(token);

        if (principal is null)
        {
            return Task.FromResult(AuthenticateResult.Fail("Token invalido o expirado."));
        }

        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
