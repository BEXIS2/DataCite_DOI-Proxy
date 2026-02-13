using LiteDB;
using MethodTimer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaelastrasz.Library.Exceptions;
using Vaelastrasz.Server.Configurations;
using Vaelastrasz.Server.Models;
using Vaelastrasz.Server.Services;

//rdy
namespace Vaelastrasz.Server.Controllers
{
    [ApiController, Authorize(Roles = "admin,user"), Route("api"), Time]
    public class PlaceholdersController : ControllerBase
    {
        private readonly ILogger<PlaceholdersController> _logger;
        private List<Admin> _admins;
        private ConnectionString _connectionString;

        public PlaceholdersController(ILogger<PlaceholdersController> logger, IConfiguration configuration, ConnectionString connectionString)
        {
            _connectionString = connectionString;
            _admins = configuration.GetSection("Admins").Get<List<Admin>>() ?? [];
            _logger = logger;
        }

        /// <summary>
        /// Löscht einen bestimmten Platzhalter anhand der angegebenen Id, wenn der aktuelle Benutzer berechtigt ist.
        /// </summary>
        /// <param name="id">Die eindeutige Id des zu löschenden Platzhalters.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem 200 OK-Status und der Antwort der Löschoperation bei Erfolg.
        /// </returns>
        /// <remarks>
        /// Diese Methode überprüft, ob der aktuelle Benutzer berechtigt ist, den Platzhalter zu löschen.
        /// Der Benutzer wird durch den UserService verifiziert, basierend auf dem bei der Anmeldung authentifizierten Benutzer.
        /// Falls der Benutzer keine Berechtigung hat den Platzhalter zu löschen, wird eine <see cref="UnauthorizedException"/> ausgelöst.
        /// Die Korrespondenz zwischen Nutzer und Platzhalter wird durch den Vergleich der User-Id sichergestellt.
        /// Achten Sie darauf, dass die richtige Benutzeridentität geladen wird, um Zugriffsrechte korrekt zu überprüfen.
        /// </remarks>
        /// <exception cref="UnauthorizedException">Wird ausgelöst, wenn der Benutzer nicht berechtigt ist, den Platzhalter zu löschen.</exception>
        [HttpDelete("placeholders/{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteByIdAsync(long id)
        {
            var placeholderService = new PlaceholderService(_connectionString);
            var placeholder = await placeholderService.GetByIdAsync(id) ?? throw new NotFoundException($"The placeholder (id: {id}) does not exist.");

            if (User.IsInRole("admin") || (User.IsInRole("user") && long.TryParse(User.FindFirst("UserId")?.Value, out long userId) && placeholder.User.Id == userId))
            {
                var response = await placeholderService.DeleteByIdAsync(id);
                return Ok(response);
            }

            return Forbid();
        }

        /// <summary>
        /// Ruft die Platzhalter des aktuell authentifizierten Benutzers ab und gibt diese im JSON-Format zurück.
        /// </summary>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem 200 OK-Status, der eine Liste von <see cref="ReadPlaceholderModel"/>-Objekten beinhaltet.
        /// </returns>
        /// <remarks>
        /// Diese Methode authentifiziert den Benutzer und ruft anschließend alle Platzhalter ab, die mit diesem Benutzer verknüpft sind.
        /// Wenn der Benutzer nicht authentifiziert ist, wird ein 403 Forbidden-Status zurückgegeben.
        /// Benutzerinformationen werden anhand des Benutzernamens über den <see cref="UserService"/> verifiziert.
        /// Die Umwandlung der abgerufenen Platzhalter erfolgt in <see cref="ReadPlaceholderModel"/>-Objekte, bevor sie an den Client zurückgegeben werden.
        /// Stellen Sie sicher, dass der Benutzer eingeloggt und die entsprechende Verbindungskonfiguration vorhanden ist.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Wird ausgelöst, wenn ein unerwarteter Fehler beim Zugriff auf Benutzerdaten auftritt.</exception>
        [HttpGet("placeholders")]
        [ProducesResponseType(typeof(IEnumerable<ReadPlaceholderModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAsync()
        {
            var placeholderService = new PlaceholderService(_connectionString);

            if (User.IsInRole("user") && long.TryParse(User.FindFirst("UserId")?.Value, out long userId))
            {
                var placeholders = (await placeholderService.GetByUserIdAsync(userId)).Select(p => ReadPlaceholderModel.Convert(p));
                return Ok(placeholders);
            }

            if (User.IsInRole("admin"))
            {
                var placeholders = (await placeholderService.GetAsync()).Select(p => ReadPlaceholderModel.Convert(p));
                return Ok(placeholders);
            }

            return Forbid();
        }

        /// <summary>
        /// Ruft einen bestimmten Platzhalter anhand der übergebenen Id ab und gibt ihn im JSON-Format zurück.
        /// </summary>
        /// <param name="id">Die eindeutige Id des abzurufenden Platzhalters.</param>
        /// <returns>
        /// Ein <see cref="IActionResult"/> mit einem 200 OK-Status, das ein <see cref="ReadPlaceholderModel"/>-Objekt enthält, das den Platzhalter repräsentiert.
        /// </returns>
        /// <remarks>
        /// Diese Methode authentifiziert den Benutzer anhand seines Benutzernamens.
        /// Der Benutzer muss eingeloggt sein, sonst wird ein 403 Forbidden-Status zurückgegeben.
        /// Der Platzhalter wird über den <see cref="PlaceholderService"/> anhand der bereitgestellten Id abgerufen.
        /// Der abgerufene Platzhalter wird in ein <see cref="ReadPlaceholderModel"/> umgewandelt, bevor er an den Client zurückgegeben wird.
        /// Stellen Sie sicher, dass die Benutzeridentität geladen ist und gültige Anmeldeinformationen vorhanden sind.
        /// </remarks>
        /// <exception cref="UnauthorizedException">Wird ausgelöst, wenn der Zugriff ohne angemessene Authentifizierung erfolgt.</exception>
        [HttpGet("placeholders/{id}")]
        [ProducesResponseType(typeof(ReadPlaceholderModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            var placeholderService = new PlaceholderService(_connectionString);
            var placeholder = await placeholderService.GetByIdAsync(id) ?? throw new NotFoundException($"The placeholder (id: {id}) does not exist.");

            if (User.IsInRole("admin") || (User.IsInRole("user") && long.TryParse(User.FindFirst("UserId")?.Value, out long userId)))
            {
                return Ok(ReadPlaceholderModel.Convert(placeholder));
            }

            return Forbid();
        }

        /// <summary>
        /// Erstellt einen neuen Platzhalter basierend auf dem bereitgestellten Modell und gibt die Details des erstellten Platzhalters zurück.
        /// </summary>
        /// <param name="model">Das <see cref="CreatePlaceholderModel"/>, das die Details des zu erstellenden Platzhalters enthält.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem 201 Created-Status, das ein <see cref="ReadPlaceholderModel"/>-Objekt enthält, das den erstellten Platzhalter repräsentiert.
        /// Die Antwort enthält auch den URI des neu erstellten Platzhalters in den HTTP-Headern.
        /// </returns>
        /// <remarks>
        /// Diese Methode authentifiziert den Benutzer und erstellt einen neuen Platzhalter unter Verwendung der bereitgestellten Daten.
        /// Der Benutzer muss eingeloggt sein, da sonst ein 403 Forbidden-Status zurückgegeben wird.
        /// Der neu erstellte Platzhalter wird in ein <see cref="ReadPlaceholderModel"/>-Objekt umgewandelt, bevor es an den Client zurückgegeben wird.
        /// Stellen Sie sicher, dass Benutzerdaten korrekt geladen sind, um Elementerstellung im Kontext des Nutzers sicherzustellen.
        /// </remarks>
        /// <exception cref="UnauthorizedException">Wird ausgelöst, wenn der Zugriff ohne ordnungsgemäße Authentifizierung erfolgt.</exception>
        [HttpPost("placeholders")]
        [ProducesResponseType(typeof(ReadPlaceholderModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PostAsync(CreatePlaceholderModel model)
        {
            if (User.IsInRole("admin") || (User.IsInRole("user") && long.TryParse(User.FindFirst("UserId")?.Value, out long userId) && model.UserId == userId))
            {
                var userService = new UserService(_connectionString);
                var placeholderService = new PlaceholderService(_connectionString);
                var id = await placeholderService.CreateAsync(model.Expression, model.RegularExpression, model.UserId);

                var placeholder = await placeholderService.GetByIdAsync(id) ?? throw new NotFoundException($"The placeholder (id: {id}) does not exist.");

                var request = HttpContext.Request;
                string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
                string resourceUrl = $"{baseUrl}/api/placeholders/{placeholder.Id}";

                return Created(resourceUrl, ReadPlaceholderModel.Convert(placeholder));
            }

            return Forbid();
        }

        /// <summary>
        /// Aktualisiert einen bestehenden Platzhalter anhand der übergebenen Id mit den bereitgestellten Daten.
        /// </summary>
        /// <param name="id">Die eindeutige Id des Platzhalters, der aktualisiert werden soll.</param>
        /// <param name="model">Das <see cref="UpdatePlaceholderModel"/>, das die neuen Daten für den Platzhalter enthält.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem 200 OK-Status, das ein <see cref="ReadPlaceholderModel"/>-Objekt des aktualisierten Platzhalters enthält.
        /// </returns>
        /// <remarks>
        /// Diese Methode erfordert die Authentifizierung des Benutzers und verifiziert, ob der aktuelle Benutzer berechtigt ist, den Platzhalter zu aktualisieren.
        /// Der Benutzer wird durch den <see cref="UserService"/> identifiziert und muss mit dem Besitzer des Platzhalters übereinstimmen.
        /// Nachdem die Berechtigungen überprüft wurden, werden die Platzhalterdaten mit den Informationen aus dem <see cref="UpdatePlaceholderModel"/> aktualisiert.
        /// Stellen Sie sicher, dass die Benutzeridentität korrekt geladen und die Benutzer-Id mit der des Platzhalters übereinstimmt.
        /// </remarks>
        /// <exception cref="UnauthorizedException">Wird ausgelöst, wenn ein Benutzer ohne die erforderlichen Berechtigungen versucht, den Platzhalter zu aktualisieren.</exception>
        [HttpPut("placeholders/{id}")]
        [ProducesResponseType(typeof(ReadPlaceholderModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PutByIdAsync(long id, UpdatePlaceholderModel model)
        {
            var placeholderService = new PlaceholderService(_connectionString);
            var placeholder = await placeholderService.GetByIdAsync(id) ?? throw new NotFoundException($"The placeholder (id: {id}) does not exist.");
            if (User.IsInRole("admin") || (User.IsInRole("user") && long.TryParse(User.FindFirst("UserId")?.Value, out long userId) && model.UserId == userId && placeholder.User.Id == userId))
            {
                var result = await placeholderService.UpdateByIdAsync(id, model.Expression, model.RegularExpression, model.UserId);
                placeholder = await placeholderService.GetByIdAsync(id);

                return Ok(ReadPlaceholderModel.Convert(placeholder));
            }

            return Forbid();
        }
    }
}