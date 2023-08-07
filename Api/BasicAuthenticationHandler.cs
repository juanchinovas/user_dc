using Application.Common.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Api
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IAuthService _authService;

        public BasicAuthenticationHandler(IAuthService authService,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _authService = authService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization")) {
                return AuthenticateResult.Fail(new AppException("Invalid credentials"));
            }

            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            if (authHeader is null)
            {
                return AuthenticateResult.Fail(new AppException("Invalid credentials"));
            }

            var credentials = Encoding.UTF8.GetString(
                Convert.FromBase64String(authHeader.Parameter)).Split(':');
            var username = credentials[0];
            var password = credentials[1];

            if (!await _authService.ValidateCredentials(username, password))
            {
                return AuthenticateResult.Fail(new AppException("Invalid credentials"));
            }

            var claims = new[] {
                new Claim(ClaimTypes.Name, username)
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
