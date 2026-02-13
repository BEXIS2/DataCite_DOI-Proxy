using LiteDB;
using MethodTimer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Vaelastrasz.Library.Exceptions;
using Vaelastrasz.Library.Models;
using Vaelastrasz.Server.Helpers;
using Vaelastrasz.Server.Services;

namespace Vaelastrasz.Server.Controllers
{
    [ApiController, Authorize(Roles = "user"), Route("api"), Time]
    public class SuffixesController : ControllerBase
    {
        private ConnectionString _connectionString;

        public SuffixesController(IConfiguration configuration, ConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Erstellt ein Suffix basierend auf dem von einem Benutzer bereitgestellten Muster und den Platzhaltern und validiert es dann.
        /// </summary>
        /// <param name="model">Das <see cref="CreateSuffixModel"/>, das die erforderlichen Daten für die Erstellung des Suffixes enthält.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem 200 OK-Status, das das erstellte und validierte Suffix im Erfolgsfall zurückgibt,
        /// oder ein 403 Forbidden-Status, wenn der Benutzer nicht berechtigt ist, oder ein BadRequestException bei ungültigem Suffix.
        /// </returns>
        /// <remarks>
        /// Diese Methode erfordert, dass der Benutzer in der Rolle "user" ist und eine authentifizierte Identität hat.
        /// Der Benutzer muss ein verknüpftes Konto und ein gültiges Muster besitzen, um das Suffix zu generieren und zu validieren.
        /// Der Suffix wird mit dem Muster des Benutzers und seinen Platzhaltern erstellt und validiert.
        /// Wenn die Validierung erfolgreich ist, wird das Suffix zurückgegeben, andernfalls wird eine Ausnahme ausgelöst.
        /// </remarks>
        /// <exception cref="BadRequestException">Wird ausgelöst, wenn das generierte Suffix ungültig ist.</exception>
        [HttpPost("suffixes")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PostAsync(CreateSuffixModel model)
        {
            if (!User.IsInRole("user") || User.Identity?.Name == null)
                return Forbid();

            var userService = new UserService(_connectionString);
            var user = await userService.GetByNameAsync(User.Identity.Name);

            if (user?.Account == null || string.IsNullOrEmpty(user.Pattern))
                return Forbid();

            // Suffix
            var suffix = SuffixHelper.Create(user.Pattern, model.Placeholders);

            // Validation
            var placeholderService = new PlaceholderService(_connectionString);

            if (SuffixHelper.Validate(suffix, user.Pattern, new Dictionary<string, string>((await placeholderService.GetByUserIdAsync(user.Id)).Select(p => new KeyValuePair<string, string>(p.Expression, p.RegularExpression)))))
                return Ok(suffix);

            //throw new BadRequestException($"The value of suffix ({suffix}) is invalid.");
            return BadRequest($"The value of suffix ({suffix}) is invalid.");
        }
    }
}