using LiteDB;
using MethodTimer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

//using Swashbuckle.AspNetCore.Annotations;
using Vaelastrasz.Library.Exceptions;
using Vaelastrasz.Library.Models;
using Vaelastrasz.Server.Configurations;
using Vaelastrasz.Server.Helpers;
using Vaelastrasz.Server.Services;

namespace Vaelastrasz.Server.Controllers
{
    [ApiController, Authorize(Roles = "user"), Route("api"), Time]
    public class DOIsController : ControllerBase
    {
        private readonly ILogger<DataCiteController> _logger;
        private List<Admin> _admins;
        private ConnectionString _connectionString;

        public DOIsController(ILogger<DataCiteController> logger, IConfiguration configuration, ConnectionString connectionString)
        {
            _connectionString = connectionString;
            _admins = configuration.GetSection("Admins").Get<List<Admin>>() ?? [];
            _logger = logger;
        }

        /// <summary>
        /// Löscht einen DOI-Eintrag aus der Applikation (NICHT aus DataCite) basierend auf dem angegebenen Präfix und Suffix, sofern der Benutzer dazu berechtigt ist.
        /// </summary>
        /// <param name="prefix">Das Präfix des DOI, der gelöscht werden soll.</param>
        /// <param name="suffix">Das Suffix des DOI, der gelöscht werden soll.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem 200 OK-Status bei erfolgreicher Löschung des DOI-Eintrags.
        /// </returns>
        /// <remarks>
        /// Diese Methode erfordert die Authentifizierung des Benutzers.
        /// Der Benutzer wird anhand seines Benutzernamens ermittelt und es wird sichergestellt, dass er berechtigt ist, den angegebenen DOI zu löschen.
        /// Ist der Benutzer nicht berechtigt, wird eine <see cref="UnauthorizedException"/> ausgelöst.
        /// Die effektive Löschung erfolgt durch den <see cref="DOIService"/>.
        /// </remarks>
        /// <exception cref="UnauthorizedException">Wird ausgelöst, wenn der Benutzer nicht die Berechtigung hat, den DOI zu löschen.</exception>
        [HttpDelete("dois/{prefix}/{suffix}")]
        public async Task<IActionResult> DeleteAsync(string prefix, string suffix)
        {
            if (!User.IsInRole("user") || User?.Identity?.Name == null)
                return Forbid();

            using var userService = new UserService(_connectionString);
            var user = await userService.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return Forbid();

            using var doiService = new DOIService(_connectionString);

            var result = await doiService.FindByPrefixAndSuffixAsync(prefix, suffix);

            if (result.User.Id != user.Id)
                throw new UnauthorizedException($"The user (id: {user.Id}) is not allowed to perform the action.");

            var response = await doiService.DeleteByIdAsync(result.Id);
            return Ok(response);
        }

        /// <summary>
        /// Ruft alle DOIs ab.
        /// </summary>
        /// <returns>
        /// Ein <see cref="Task{I}"/> mit einem 200 OK-Status und einer Liste von <see cref="ReadDOIModel"/>-Objekten,
        /// die die DOI-Einträge des Benutzers darstellen.
        /// </returns>
        /// <remarks>
        /// Diese Methode erfordert die Authentifizierung des Benutzers.
        /// Es werden nur die DOI-Einträge zurückgegeben, die mit dem authentifizierten Benutzer verknüpft sind.
        /// Wenn der Benutzer nicht authentifiziert ist, wird ein 403 Forbidden-Status zurückgegeben.
        /// Die DOI-Daten werden über den <see cref="DOIService"/> abgerufen und in lesefreundliche Modelle konvertiert.
        /// </remarks>
        /// <exception cref="UnauthorizedAccessException">Wird ausgelöst, wenn die Benutzeridentität nicht verfügbar ist.</exception>
        [HttpGet("dois")]
        public async Task<IActionResult> GetAsync()
        {
            if (!User.IsInRole("user") || User?.Identity?.Name == null)
                return Forbid();

            using var userService = new UserService(_connectionString);
            var user = await userService.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return Forbid();

            using var doiService = new DOIService(_connectionString);
            var dois = (await doiService.FindByUserIdAsync(user.Id)).Select(d => ReadDOIModel.Convert(d));

            return Ok(dois);
        }

        /// <summary>
        /// Ruft einen spezifischen DOI-Eintrag basierend auf der übergebenen Id ab und gibt ihn im JSON-Format zurück.
        /// </summary>
        /// <param name="id">Die eindeutige Id des DOI-Eintrags, der abgerufen werden soll.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem 200 OK-Status und einem <see cref="ReadDOIModel"/>-Objekt,
        /// das die Details des DOI-Eintrags darstellt.
        /// </returns>
        /// <remarks>
        /// Diese Methode erfordert die Authentifizierung des Benutzers.
        /// Es wird überprüft, ob der Benutzer berechtigt ist, auf den angeforderten DOI-Eintrag zuzugreifen.
        /// Wenn der Benutzer nicht identifiziert werden kann, wird ein 403 Forbidden-Status zurückgegeben.
        /// Sollte der Benutzer nicht die Berechtigung für den Zugriff auf den DOI-Eintrag haben, wird eine <see cref="UnauthorizedException"/> ausgelöst.
        /// </remarks>
        /// <exception cref="UnauthorizedException">Wird ausgelöst, wenn der Benutzer nicht die Berechtigung hat, auf den DOI-Eintrag zuzugreifen.</exception>
        [HttpGet("dois/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            if (!User.IsInRole("user") || User?.Identity?.Name == null)
                return Forbid();

            using var userService = new UserService(_connectionString);
            var user = await userService.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return Forbid();

            using var doiService = new DOIService(_connectionString);
            var doi = await doiService.FindByIdAsync(id);

            if (doi.User.Id != user.Id)
                throw new UnauthorizedException($"The user (id: {user.Id}) is not allowed to perform the action.");

            return Ok(ReadDOIModel.Convert(doi));
        }

        /// <summary>
        /// Ruft einen spezifischen DOI-Eintrag basierend auf dem übergebenen Präfix und Suffix ab und gibt ihn im JSON-Format zurück.
        /// </summary>
        /// <param name="prefix">Das Präfix des DOI, der abgerufen werden soll.</param>
        /// <param name="suffix">Das Suffix des DOI, der abgerufen werden soll.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem 200 OK-Status und einem <see cref="ReadDOIModel"/>-Objekt,
        /// das die Details des DOI-Eintrags darstellt.
        /// </returns>
        /// <remarks>
        /// Diese Methode erfordert die Authentifizierung des Benutzers.
        /// Es wird sichergestellt, dass der aktuelle Benutzer berechtigt ist, auf den angeforderten DOI-Eintrag zuzugreifen.
        /// Wenn der Benutzer nicht authentifiziert ist, wird ein 403 Forbidden-Status zurückgegeben.
        /// Sollte der Benutzer nicht die Berechtigung für den Zugriff auf den DOI-Eintrag haben, wird eine <see cref="UnauthorizedException"/> ausgelöst.
        /// </remarks>
        /// <exception cref="UnauthorizedException">Wird ausgelöst, wenn der Benutzer nicht die Berechtigung hat, auf den DOI-Eintrag zuzugreifen.</exception>
        [HttpGet("dois/{prefix}/{suffix}")]
        public async Task<IActionResult> GetByPrefixAndSuffix(string prefix, string suffix)
        {
            if (!User.IsInRole("user") || User?.Identity?.Name == null)
                return Forbid();

            using var userService = new UserService(_connectionString);
            var user = await userService.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return Forbid();

            using var doiService = new DOIService(_connectionString);
            var result = await doiService.FindByPrefixAndSuffixAsync(prefix, suffix);

            if (result.User.Id != user.Id)
                throw new UnauthorizedException($"The user (id: {user.Id}) is not allowed to perform the action..");

            return Ok(ReadDOIModel.Convert(result));
        }

        [HttpPost("dois")]
        public async Task<IActionResult> Post(CreateDOIModel model)
        {
            if (!User.IsInRole("user") || User?.Identity?.Name == null)
                return Forbid();

            using var userService = new UserService(_connectionString);
            var user = await userService.FindByNameAsync(User.Identity.Name);

            if (user == null || user.Account == null || string.IsNullOrEmpty(user.Account.Prefix) || string.IsNullOrEmpty(user.Pattern))
                return Forbid();

            using var placeholderService = new PlaceholderService(_connectionString);
            var placeholders = await placeholderService.FindByUserIdAsync(user.Id);

            if (!DOIHelper.Validate($"{model.Prefix}/{model.Suffix}", user.Account.Prefix, user.Pattern, new Dictionary<string, string>(placeholders.Select(p => new KeyValuePair<string, string>(p.Expression, p.RegularExpression)))))
                throw new ForbiddenException($"The doi (prefix: {model.Prefix}, suffix: {model.Suffix}) is invalid.");

            using var doiService = new DOIService(_connectionString);
            var id = await doiService.CreateAsync(model.Prefix, model.Suffix, DOIStateType.Draft, user.Id, "");
            var doi = await doiService.FindByIdAsync(id);

            var request = HttpContext.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            string resourceUrl = $"{baseUrl}/api/dois/{doi.Id}";

            return Created(resourceUrl, ReadDOIModel.Convert(doi));
        }

        [HttpPut("dois/{doi}")]
        public async Task<IActionResult> PutByDOI(string doi, UpdateDOIModel model)
        {
            if (!User.IsInRole("user") || User?.Identity?.Name == null)
                return Forbid();

            using var userService = new UserService(_connectionString);
            var user = await userService.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return Forbid();

            using var doiService = new DOIService(_connectionString);
            var _doi = await doiService.FindByDOIAsync(doi);

            if (_doi.User.Id != user.Id)
                throw new UnauthorizedException($"The user (id: {user.Id}) is not allowed to perform the action.");

            var result = await doiService.UpdateByIdAsync(_doi.Id, model.State, model.Value);
            _doi = await doiService.FindByDOIAsync(doi);
            return Ok(ReadDOIModel.Convert(_doi));
        }

        [HttpPut("dois/{prefix}/{suffix}")]
        public async Task<IActionResult> PutByPrefixAndSuffix(string prefix, string suffix, UpdateDOIModel model)
        {
            if (!User.IsInRole("user") || User?.Identity?.Name == null)
                return Forbid();

            using var userService = new UserService(_connectionString);
            var user = await userService.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return Forbid();

            using var doiService = new DOIService(_connectionString);
            var doi = await doiService.FindByPrefixAndSuffixAsync(prefix, suffix);

            if (doi.User.Id != user.Id)
                throw new UnauthorizedException($"The user (id: {user.Id}) is not allowed to perform the action.");

            var result = await doiService.UpdateByPrefixAndSuffixAsync(prefix, suffix, model.State, model.Value);
            doi = await doiService.FindByPrefixAndSuffixAsync(prefix, suffix);
            return Ok(ReadDOIModel.Convert(doi));
        }
    }
}