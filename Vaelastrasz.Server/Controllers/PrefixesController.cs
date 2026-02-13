using LiteDB;
using MethodTimer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaelastrasz.Server.Services;

//rdy
namespace Vaelastrasz.Server.Controllers
{
    [ApiController, Authorize(Roles = "user"), Route("api"), Time]
    public class PrefixesController : ControllerBase
    {
        private ConnectionString _connectionString;

        public PrefixesController(IConfiguration configuration, ConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Ruft das Präfix des authentifizierten Benutzers ab, sofern dieser über die erforderlichen Berechtigungen verfügt.
        /// </summary>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem 200 OK-Status und dem Präfix als Zeichenfolge, wenn der Abruf erfolgreich ist,
        /// oder ein 403 Forbidden-Status, wenn der Benutzer nicht berechtigt ist.
        /// </returns>
        /// <remarks>
        /// Diese Methode setzt voraus, dass der Benutzer in der Rolle "user" authentifiziert ist.
        /// Ein Benutzer ohne ein verknüpftes Konto oder ohne definiertes Präfix wird mit einem 403 Forbidden-Status abgewiesen.
        /// Die Methode extrahiert das Präfix aus dem Benutzerkonto und gibt es zurück.
        /// Entwicklern wird empfohlen, sicherzustellen, dass die erforderliche Benutzerrolle und die Konto-Konfiguration korrekt sind,
        /// um eine erfolgreiche Anfrage zu ermöglichen.
        /// </remarks>
        [HttpGet("prefixes")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAsync()
        {
            if (!User.IsInRole("user") || User.Identity?.Name == null)
                return Forbid();

            var userService = new UserService(_connectionString);
            var user = await userService.GetByNameAsync(User.Identity.Name);

            if (user?.Account == null || string.IsNullOrEmpty(user.Account.Prefix))
                return Forbid();

            // Prefix
            var prefix = user.Account.Prefix;

            return Ok(prefix);
        }
    }
}