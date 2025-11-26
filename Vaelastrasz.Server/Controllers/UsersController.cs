using LiteDB;
using MethodTimer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

//using Swashbuckle.AspNetCore.Annotations;
using Vaelastrasz.Server.Configurations;
using Vaelastrasz.Server.Models;
using Vaelastrasz.Server.Services;

namespace Vaelastrasz.Server.Controllers
{
    [ApiController, Route("api"), Authorize(Roles = "admin"), Time]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private List<Admin> _admins;
        private ConnectionString _connectionString;

        public UsersController(ILogger<UsersController> logger, IConfiguration configuration, ConnectionString connectionString)
        {
            _connectionString = connectionString;
            _admins = configuration.GetSection("Admins").Get<List<Admin>>() ?? [];   
            _logger = logger;
        }

        /// <summary>
        /// Löscht einen Benutzer basierend auf der übergebenen Id, sofern der aktuelle Benutzer über die erforderlichen Berechtigungen verfügt.
        /// </summary>
        /// <param name="id">Die Id des Benutzers, der gelöscht werden soll.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem 200 OK-Status, bei erfolgreicher Löschung des Benutzers,
        /// oder ein 403 Forbidden-Status, wenn der aktuelle Benutzer nicht die erforderlichen Administratorrechte besitzt.
        /// </returns>
        /// <remarks>
        /// Diese Methode setzt voraus, dass der aktuelle Benutzer die Rolle "admin" innehat.
        /// Ohne angemessene Admin-Berechtigungen wird der Zugriff mit einem 403 Forbidden-Status abgewiesen.
        /// Die Löschung wird über den <see cref="UserService"/> durchgeführt, der den Benutzer anhand der übergebenen Id aus dem System entfernt.
        /// Entwickler sollten sicherstellen, dass die erforderlichen Administratorrechte gewährt sind, um die Löschoperation erfolgreich ausführen zu können.
        /// </remarks>
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            if (!User.IsInRole("admin"))
                return Forbid();

            using var userService = new UserService(_connectionString);
            var response = await userService.DeleteByIdAsync(id);

            return Ok(response);
        }

        /// <summary>
        /// Ruft eine Liste aller Benutzer ab, sofern der aktuelle Benutzer über die erforderlichen Berechtigungen verfügt.
        /// </summary>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem 200 OK-Status und einer Liste von <see cref="ReadUserModel"/>-Objekten,
        /// die die Benutzer des Systems darstellen. Gibt einen 403 Forbidden-Status zurück, wenn der aktuelle Benutzer nicht die
        /// erforderlichen Administratorrechte besitzt.
        /// </returns>
        /// <remarks>
        /// Diese Methode setzt voraus, dass der aktuelle Benutzer die Rolle "admin" innehat.
        /// Ohne angemessene Admin-Berechtigungen wird der Zugriff mit einem 403 Forbidden-Status abgewiesen.
        /// Die Benutzerliste wird über den <see cref="UserService"/> abgerufen und in lesbare Modelle konvertiert.
        /// Entwickler sollten sicherstellen, dass die erforderlichen Administratorrechte gewährt sind, um die Liste der Benutzer abzurufen.
        /// </remarks>
        [HttpGet("users")]
        public async Task<IActionResult> GetAsync()
        {
            if (!User.IsInRole("admin"))
                return Forbid();

            using var userService = new UserService(_connectionString);
            var users = await userService.FindAsync();

            return Ok(new List<ReadUserModel>(users.Select(u => ReadUserModel.Convert(u))));
        }

        /// <summary>
        /// Ruft die Details eines bestimmten Benutzers basierend auf der übergebenen Id ab.
        /// </summary>
        /// <param name="id">Die eindeutige Id des Benutzers, dessen Details abgerufen werden sollen.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem 200 OK-Status und einem <see cref="ReadUserModel"/>-Objekt,
        /// das die Details des Benutzers darstellt, oder ein 403 Forbidden-Status, wenn der aktuelle Benutzer nicht die
        /// erforderlichen Administratorrechte besitzt.
        /// </returns>
        /// <remarks>
        /// Diese Methode setzt voraus, dass der aktuelle Benutzer die Rolle "admin" innehat.
        /// Wenn dem Benutzer die erforderlichen Admin-Berechtigungen fehlen, wird der Zugriff mit einem 403 Forbidden-Status abgewiesen.
        /// Die Benutzerdetails werden über den <see cref="UserService"/> abgerufen und in ein lesbares Modell konvertiert.
        /// Entwickler sollten sicherstellen, dass die Adminrechte korrekt zugewiesen sind, um Zugriff auf die Benutzerdetails zu gewähren.
        /// </remarks>
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            if (!User.IsInRole("admin"))
                return Forbid();

            using var userService = new UserService(_connectionString);
            var user = await userService.FindByIdAsync(id);

            return Ok(ReadUserModel.Convert(user));
        }

        /// <summary>
        /// Erstellt einen neuen Benutzer basierend auf den bereitgestellten Modelldaten.
        /// </summary>
        /// <param name="model">Das <see cref="CreateUserModel"/>, das die Details des zu erstellenden Benutzers enthält.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem 201 Created-Status und einem <see cref="ReadUserModel"/>-Objekt,
        /// das die Details des neu erstellten Benutzers darstellt. Gibt ein 403 Forbidden-Status zurück, wenn der aktuelle
        /// Benutzer nicht die erforderlichen Administratorrechte besitzt, oder ein 400 Bad Request-Status, wenn die Erstellung
        /// des Benutzers fehlschlägt.
        /// </returns>
        /// <remarks>
        /// Diese Methode setzt voraus, dass der aktuelle Benutzer die Rolle "admin" innehat.
        /// Ohne die erforderlichen Admin-Berechtigungen wird der Zugriff mit einem 403 Forbidden-Status abgewiesen.
        /// Die Benutzerdetails werden über den <see cref="UserService"/> erstellt und anschließend abgerufen, um sie zurückzugeben.
        /// Wenn die Erstellung nicht erfolgreich ist, wird ein 400 Bad Request-Status zurückgegeben.
        /// Entwickler sollten sicherstellen, dass das Erstellungsmodell alle erforderlichen Felder ausfüllt und die
        /// Admin-Berechtigungen korrekt zugewiesen sind, um die Benutzerdetails erfolgreich zu erstellen.
        /// </remarks>
        [HttpPost("users")]
        public async Task<IActionResult> PostAsync(CreateUserModel model)
        {
            if (!User.IsInRole("admin"))
                return Forbid();

            using var userService = new UserService(_connectionString);

            var id = await userService.CreateAsync(model.Name, model.Password, model.Project, model.Pattern, model.AccountId, true);
            var user = await userService.FindByIdAsync(id);

            if (user == null)
                return BadRequest();

            var request = HttpContext.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            string resourceUrl = $"{baseUrl}/api/users/{user.Id}";

            return Created(resourceUrl, ReadUserModel.Convert(user));
        }

        /// <summary>
        /// Aktualisiert die Details eines bestehenden Benutzers basierend auf der übergebenen Id und den neuen Modelldaten.
        /// </summary>
        /// <param name="id">Die eindeutige Id des Benutzers, der aktualisiert werden soll.</param>
        /// <param name="model">Das <see cref="UpdateUserModel"/>, das die neuen Benutzerdaten enthält.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem 200 OK-Status und dem aktualisierten <see cref="ReadUserModel"/>-Objekt,
        /// das die Details des Benutzers darstellt. Gibt ein 403 Forbidden-Status zurück, wenn der aktuelle Benutzer nicht die
        /// erforderlichen Administratorrechte besitzt, oder ein 400 Bad Request-Status, wenn der Benutzer nicht gefunden wird
        /// oder die Aktualisierung fehlschlägt.
        /// </returns>
        /// <remarks>
        /// Diese Methode erfordert, dass der aktuelle Benutzer die Rolle "admin" innehat.
        /// Wenn dem Benutzer die erforderlichen Admin-Berechtigungen fehlen, wird der Zugriff mit einem 403 Forbidden-Status abgewiesen.
        /// Die Benutzerdetails werden mit Hilfe des <see cref="UserService"/> aktualisiert.
        /// Wenn die Aktualisierung erfolgreich ist, werden die aktualisierten Benutzerdetails zurückgegeben.
        /// Entwickler sollten sicherstellen, dass alle erforderlichen Felder im Update-Modell korrekt ausgefüllt sind
        /// und die Adminberechtigungen korrekt zugewiesen sind, um die Aktualisierung erfolgreich durchzuführen.
        /// </remarks>
        [HttpPut("users/{id}")]
        public async Task<IActionResult> PutByIdAsync(long id, UpdateUserModel model)
        {
            if (!User.IsInRole("admin"))
                return Forbid();

            using var userService = new UserService(_connectionString);

            var result = await userService.UpdateByIdAsync(id, model.Name, model.Password, model.Project, model.Pattern, model.AccountId, model.IsActive);
            var user = await userService.FindByIdAsync(id);

            if (user == null)
                return BadRequest();

            return Ok(ReadUserModel.Convert(user));
        }
    }
}