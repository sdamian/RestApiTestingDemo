using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebApplication.Tests.Authentication
{
    public class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationSchemeOptions>
    {
        public TestAuthenticationHandler(
            IOptionsMonitor<TestAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (this.Options.ShouldBeAuthenticated)
            {
                var authenticationTicket = new AuthenticationTicket(
                    new ClaimsPrincipal(
                        new GenericPrincipal(
                            new GenericIdentity("Test User", "Test Identity"), new string[0])),
                    new AuthenticationProperties(),
                    "Test Scheme");

                return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
            }

            return Task.FromResult(AuthenticateResult.Fail("Denied"));
        }
    }

    public class TestAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public bool ShouldBeAuthenticated { get; set; }
    }
}
