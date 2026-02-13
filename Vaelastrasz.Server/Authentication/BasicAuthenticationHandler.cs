using LiteDB;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Vaelastrasz.Library.Exceptions;
using Vaelastrasz.Library.Extensions;
using Vaelastrasz.Server.Configurations;
using Vaelastrasz.Server.Services;

namespace Vaelastrasz.Server.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private List<Admin> _admins;
        private ConnectionString _connectionString;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IConfiguration configuration,
            ConnectionString connectionString) : base(options, logger, encoder)
        {
            _connectionString = connectionString;
            _admins = configuration.GetSection("Admins").Get<List<Admin>>() ?? [];
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                return AuthenticateResult.NoResult();
            }

            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader != null && authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("basic ".Length).Trim();
                // in general latin1 instead of utf8?
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(token)).Split(':');

                //
                // Admin
                var admin = _admins.Find(a => a.Name.Equals(credentials[0]));

                if (admin != null && admin.Password.Equals(credentials[1]))
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, credentials[0]),
                        new Claim(ClaimTypes.Role, "admin"),
                        //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    var identity = new ClaimsIdentity(claims, "Basic");
                    var claimsPrincipal = new ClaimsPrincipal(identity);
                    return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
                }

                var userService = new UserService(_connectionString);

                if (await userService.ExistsByNameAsync(credentials[0]) && await userService.VerifyAsync(credentials[0], credentials[1]))
                {
                    var user = userService.GetByNameAsync(credentials[0]).Result;
                    var accountType = EnumExtensions.GetEnumMemberValue(user.Account.AccountType);

                    var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.Name, credentials[0]),
                            new Claim("UserId", user.Id.ToString()),
                            new Claim(ClaimTypes.Role, $"user"),
                            new Claim(ClaimTypes.Role, $"user-{accountType}"),
                        };

                    var identity = new ClaimsIdentity(claims, "Basic");
                    var claimsPrincipal = new ClaimsPrincipal(identity);

                    return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
                }

                Response.StatusCode = 401;
                Response.Headers.Append("www-authenticate", "Basic Authorization");
                return AuthenticateResult.Fail(new UnauthorizedException());
            }
            else
            {
                Response.StatusCode = 401;
                Response.Headers.Append("www-authenticate", "Basic Authorization");
                return AuthenticateResult.Fail(new UnauthorizedException());
            }
        }
    }
}