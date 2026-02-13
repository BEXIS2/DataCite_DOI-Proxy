using LiteDB;
using MethodTimer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Vaelastrasz.Library.Exceptions;
using Vaelastrasz.Library.Extensions;
using Vaelastrasz.Library.Models;
using Vaelastrasz.Server.Attributes;
using Vaelastrasz.Server.Helpers;
using Vaelastrasz.Server.Services;

namespace Vaelastrasz.Server.Controllers
{
    [ApiController, Authorize(Roles = "user-datacite"), Route("api"), Time]
    public class DataCiteController : ControllerBase
    {
        private readonly ILogger<DataCiteController> _logger;
        private ConnectionString _connectionString;

        public DataCiteController(ILogger<DataCiteController> logger, IConfiguration configuration, ConnectionString connectionString)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        /// <summary>
        /// Löscht einen DataCite-Eintrag anhand der spezifischen DOI (Digital Object Identifier).
        /// </summary>
        /// <param name="doi">Der Digital Object Identifier (DOI) des DataCite-Eintrags, der gelöscht werden soll.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit dem HTTP-Statuscode des Löschvorgangs.
        /// Bei Erfolg wird der Statuscode der Antwort des externen DOI-Dienstes zurückgegeben.
        /// </returns>
        /// <remarks>
        /// Diese Methode authentifiziert den Benutzer und erhält die zugehörigen Kontoinformationen, um eine Verbindung mit dem DOI-Dienst herzustellen.
        /// Sie sendet eine DELETE-Anfrage an die API des DOI-Dienstes.
        /// Wenn die Antwort erfolgreich ist, wird der Eintrag auch lokal über den <see cref="DOIService"/> gelöscht.
        /// Es ist sicherzustellen, dass der Benutzer authentifiziert ist und ein gültiges Konto mit den entsprechenden Berechtigungen besitzt.
        /// </remarks>
        /// <exception cref="NotFoundException">Wird ausgelöst, wenn das Konto des aktuellen Benutzers nicht vorhanden ist.</exception>
        [HttpDelete("datacite/{doi}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteByDOIAsync(string doi)
        {
            if (!User.IsInRole("user-datacite") || User?.Identity?.Name == null)
                return Forbid();

            var userService = new UserService(_connectionString);
            var user = await userService.GetByNameAsync(User.Identity.Name);

            if (user == null || user?.Account == null || string.IsNullOrEmpty(user.Account.Name) || string.IsNullOrEmpty(user.Account.Password))
                return Forbid();

            var client = new HttpClient
            {
                BaseAddress = new Uri(user.Account.Host),
                DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.Account.Name}:{user.Account.Password}"))) }
            };

            var response = await client.DeleteAsync($"dois/{doi}");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            var doiService = new DOIService(_connectionString);
            await doiService.DeleteByDOIAsync(doi);
            return StatusCode((int)response.StatusCode, JsonConvert.DeserializeObject<ReadDataCiteModel>(await response.Content.ReadAsStringAsync()));
        }

        /// <summary>
        /// Ruft eine Liste von DataCite-Einträgen für den aktuell authentifizierten Benutzer ab und gibt sie im JSON-Format zurück.
        /// </summary>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem 200 OK-Status, das eine Liste von <see cref="ReadDataCiteModel"/>-Objekten enthält,
        /// die die DataCite-Einträge des Benutzers darstellen. Wenn keine Einträge vorhanden sind, wird eine leere Liste zurückgegeben.
        /// </returns>
        /// <remarks>
        /// Diese Methode erfordert eine Benutzerauthentifizierung und setzt voraus, dass der Benutzer ein verknüpftes Konto besitzt.
        /// Über den <see cref="DOIService"/> werden alle DOIs des Benutzers abgerufen, und anschließend wird für jeden DOI eine Anfrage an die
        /// externe DOI-API gesendet, um die vollständigen Details zu erhalten.
        /// Der Benutzername und das Passwort werden zur Authentifizierung gegenüber der externen API verwendet.
        /// Stellen Sie sicher, dass die Account-Daten vorhanden sind, um eine erfolgreiche API-Kommunikation sicherzustellen.
        /// </remarks>
        /// <exception cref="NotFoundException">Wird ausgelöst, wenn das Konto des aktuellen Benutzers nicht existiert.</exception>
        [HttpGet("datacite")]
        [ProducesResponseType(typeof(List<ReadDataCiteModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAsync()
        {
            if (!User.IsInRole("user-datacite") || User?.Identity?.Name == null)
                return Forbid();

            var userService = new UserService(_connectionString);
            var user = await userService.GetByNameAsync(User.Identity.Name);

            if (user == null || user?.Account == null || string.IsNullOrEmpty(user.Account.Name) || string.IsNullOrEmpty(user.Account.Password))
                return Forbid();

            var doiService = new DOIService(_connectionString);
            var dois = await doiService.GetByUserIdAsync(user.Id);

            var result = new List<ReadDataCiteModel>();

            if (dois.Count == 0)
                return Ok(result);

            var client = new HttpClient
            {
                BaseAddress = new Uri(user.Account.Host),
                DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.Account.Name}:{user.Account.Password}"))) }
            };

            foreach (var doi in dois)
            {
                var response = await client.GetAsync($"dois/{doi}?publisher=true&affiliation=true");

                if (response == null)
                    continue;

                if (!response.IsSuccessStatusCode)
                    continue;

                if (response.Content == null)
                    continue;

                var item = JsonConvert.DeserializeObject<ReadDataCiteModel>(await response.Content.ReadAsStringAsync());

                if (item != null)
                    result.Add(item);
            }

            return Ok(result);
        }

        /// <summary>
        /// Ruft einen DataCite-Eintrag basierend auf dem gegebenen Präfix und Suffix ab, sofern der Benutzer dazu berechtigt ist.
        /// </summary>
        /// <param name="prefix">Das Präfix des DOIs, das abgerufen werden soll.</param>
        /// <param name="suffix">Das Suffix des DOIs, das abgerufen werden soll.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit dem HTTP-Statuscode der Operation.
        /// Bei Erfolg wird ein <see cref="ReadDataCiteModel"/>-Objekt zurückgegeben, das die Details des DataCite-Eintrags darstellt.
        /// </returns>
        /// <remarks>
        /// Diese Methode stellt sicher, dass der aktuelle Benutzer autorisiert ist, den angeforderten DOI-Datensatz aufzurufen.
        /// Sie überprüft die Benutzerzugehörigkeit zum abgerufenem DOI-Datensatz und fordert den externen DOI-Dienst mit den angegebenen
        /// Präfix- und Suffix-Parametern ab, um die vollständigen Datensatzdetails zu erhalten.
        /// Die Methode verwendet eine Basisauthentifizierung, wenn die Account-Informationen des Benutzers vorhanden sind.
        /// </remarks>
        /// <exception cref="NotFoundException">Wird ausgelöst, wenn das Konto des Benutzers nicht existiert.</exception>
        /// <exception cref="UnauthorizedException">Wird ausgelöst, wenn der Benutzer nicht berechtigt ist, den DOI-Datensatz zu sehen.</exception>
        [HttpGet("datacite/{prefix}/{suffix}")]
        [ProducesResponseType(typeof(ReadDataCiteModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByPrefixAndSuffixAsync(string prefix, string suffix)
        {
            if (!User.IsInRole("user-datacite") || User?.Identity?.Name == null)
                return Forbid();

            var userService = new UserService(_connectionString);
            var user = await userService.GetByNameAsync(User.Identity!.Name!);

            if (user == null || user?.Account == null || string.IsNullOrEmpty(user.Account.Name) || string.IsNullOrEmpty(user.Account.Password))
                return Forbid();

            var doiService = new DOIService(_connectionString);
            var doi = await doiService.GetByPrefixAndSuffixAsync(prefix, suffix);

            if (doi.User.Id != user.Id)
                throw new UnauthorizedException($"The user (id: {user.Id}) is not allowed to perform the action.");

            var client = new HttpClient
            {
                BaseAddress = new Uri(user.Account.Host),
                DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.Account.Name}:{user.Account.Password}"))) }
            };

            var response = await client.GetAsync($"dois/{prefix}/{suffix}?publisher=true&affiliation=true");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            return StatusCode((int)response.StatusCode, JsonConvert.DeserializeObject<ReadDataCiteModel>(await response.Content.ReadAsStringAsync()));
        }

        /// <summary>
        /// Ruft die Zitierstil-Informationen für einen spezifischen DataCite-Eintrag anhand des Präfixes und Suffixes ab.
        /// </summary>
        /// <param name="prefix">Das des DOIs für den entsprechenden DataCite-Eintrag.</param>
        /// <param name="suffix">Das Suffix des DOIs für den entsprechenden DataCite-Eintrag.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem HTTP-Statuscode und den Zitierstil-Informationen im <c>text/x-bibliography</c> Format.
        /// </returns>
        /// <remarks>
        /// Diese Methode erfordert die Authentifizierung und Authentisierung des Benutzers und überprüft dessen Besitzrechte an dem angeforderten DOI.
        /// Der Zitierstil wird über den benutzerdefinierten HTTP-Header <c>X-Citation-Style</c> spezifiziert, der folgende Werte akzeptiert:
        /// "apa", "harvard-cite-them-right", "modern-language-association", "vancouver", "chicago-fullnote-bibliography", "ieee".
        /// Die Methode ruft den externen DOI-Dienst auf, um die Bibliographie-Informationen im angegebenen Stil zu erhalten.
        /// </remarks>
        /// <exception cref="NotFoundException">Wird ausgelöst, wenn das Konto des Benutzers nicht existiert.</exception>
        /// <exception cref="UnauthorizedException">Wird ausgelöst, wenn der Benutzer nicht berechtigt ist, Informationen zum DOI abzurufen.</exception>
        [HttpGet("datacite/{prefix}/{suffix}/citations")]
        [SwaggerCustomHeader("X-Citation-Style", ["apa", "harvard-cite-them-right", "modern-language-association", "vancouver", "chicago-fullnote-bibliography", "ieee"])]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetCitationStyleByPrefixAndSuffixAsync(string prefix, string suffix)
        {
            if (!User.IsInRole("user-datacite") || User?.Identity?.Name == null)
                return Forbid();

            var userService = new UserService(_connectionString);
            var user = await userService.GetByNameAsync(User.Identity.Name!);

            if (user == null || user?.Account == null || string.IsNullOrEmpty(user.Account.Name) || string.IsNullOrEmpty(user.Account.Password))
                return Forbid();

            var doiService = new DOIService(_connectionString);
            var doi = await doiService.GetByPrefixAndSuffixAsync(prefix, suffix);

            if (doi.User.Id != user.Id)
                throw new UnauthorizedException($"The user (id: {user.Id}) is not allowed to perform the action.");

            var client = new HttpClient
            {
                BaseAddress = new Uri(user.Account.Host),
                DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.Account.Name}:{user.Account.Password}"))) }
            };

            var request = new HttpRequestMessage(HttpMethod.Get, $"dois/{prefix}/{suffix}?style={Request.Headers["X-Citation-Style"].ToString()}");
            request.Headers.Add("Accept", "text/x-bibliography");

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Ruft die Metadaten für einen spezifischen DataCite-Eintrag basierend auf dem gegebenen Präfix und Suffix im angegebenen Format ab.
        /// </summary>
        /// <param name="prefix">Das Präfix des DOI für den entsprechenden DataCite-Eintrag.</param>
        /// <param name="suffix">Das Suffix des DOI für den entsprechenden DataCite-Eintrag.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit dem HTTP-Statuscode der Anfrage.
        /// Bei Erfolg wird der Inhalt der Metadaten im vom Client angeforderten Format zurückgegeben.
        /// </returns>
        /// <remarks>
        /// Diese Methode erfordert die Authentifizierung und Authentisierung des Benutzers und prüft, ob der Benutzer berechtigt ist, den angeforderten DOI abzurufen.
        /// Der Metadateninhalt wird über einen benutzerdefinierten HTTP-Header <c>X-Metadata-Format</c> spezifiziert, der folgende Werte akzeptiert:
        /// "application/x-research-info-systems", "application/x-bibtex", "application/vnd.jats+xml", "application/vnd.codemeta.ld+json",
        /// "application/vnd.citationstyles.csl+json", "application/vnd.schemaorg.ld+json", "application/vnd.datacite.datacite+json", "application/vnd.datacite.datacite+xml".
        /// Die Methode nutzt die externe DOI-API, um die Metadaten im angegebenen Format zu erhalten.
        /// </remarks>
        /// <exception cref="NotFoundException">Wird ausgelöst, wenn das Konto des Benutzers nicht existiert.</exception>
        /// <exception cref="UnauthorizedException">Wird ausgelöst, wenn der Benutzer nicht berechtigt ist, Metadaten für den DOI abzurufen.</exception>
        [HttpGet("datacite/{prefix}/{suffix}/metadata")]
        [SwaggerCustomHeader("X-Metadata-Format", ["application/x-research-info-systems", "application/x-bibtex", "application/vnd.jats+xml", "application/vnd.codemeta.ld+json", "application/vnd.citationstyles.csl+json", "application/vnd.schemaorg.ld+json", "application/vnd.datacite.datacite+json", "application/vnd.datacite.datacite+xml"])]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetMetadataFormatByPrefixAndSuffixAsync(string prefix, string suffix)
        {
            if (!User.IsInRole("user-datacite") || User?.Identity?.Name == null)
                return Forbid();

            var userService = new UserService(_connectionString);
            var user = await userService.GetByNameAsync(User.Identity!.Name!);

            if (user == null || user?.Account == null || string.IsNullOrEmpty(user.Account.Name) || string.IsNullOrEmpty(user.Account.Password))
                return Forbid();

            var doiService = new DOIService(_connectionString);
            var doi = await doiService.GetByPrefixAndSuffixAsync(prefix, suffix);

            if (doi.User.Id != user.Id)
                throw new UnauthorizedException($"The user (id: {user.Id}) is not allowed to perform the action.");

            var client = new HttpClient
            {
                BaseAddress = new Uri(user.Account.Host),
                DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.Account.Name}:{user.Account.Password}"))) }
            };

            var request = new HttpRequestMessage(HttpMethod.Get, $"dois/{prefix}/{suffix}");
            request.Headers.Add("Accept", Request.Headers["X-Metadata-Format"].ToString());

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Erstellt einen neuen DataCite-Eintrag basierend auf den bereitgestellten Modelldaten.
        /// </summary>
        /// <param name="model">Das <see cref="CreateDataCiteModel"/>, das die erforderlichen Daten für die Erstellung des DataCite-Eintrags enthält.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem 201 Created-Status und einem <see cref="ReadDataCiteModel"/>-Objekt,
        /// das die Details des erstellten DataCite-Eintrags enthält.
        /// </returns>
        /// <remarks>
        /// Diese Methode erfordert, dass der Benutzer authentifiziert ist und ein gültiges Konto besitzt.
        /// Die Methode überprüft die Gültigkeit des DOI anhand von Platzhaltern, die für den Benutzer definiert sind.
        /// Nach erfolgreicher Validierung des DOI sendet sie eine Anfrage an einen externen DOI-Dienst, um den Eintrag zu erstellen.
        /// Die Anfrage auf fehlerfreie Ausführung wird über HTTP-Statuscodes überwacht, und bei Misserfolg wird der Eintrag lokal zurückabgerollt.
        /// Der Zugriff erfolgt über eine abgesicherte Verbindung, die durch die Bereitstellung von Authentifizierungsdetails im Header gewährleistet wird.
        /// </remarks>
        /// <exception cref="NotFoundException">Wird ausgelöst, wenn das Konto des Benutzers nicht existiert.</exception>
        /// <exception cref="ForbiddenException">Wird ausgelöst, wenn der DOI ungültig ist oder nicht verwaltet werden kann.</exception>
        [HttpPost("datacite")]
        [ProducesResponseType(typeof(ReadDataCiteModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PostAsync(CreateDataCiteModel model)
        {
            if (!User.IsInRole("user-datacite") || User?.Identity?.Name == null)
                return Forbid();

            var userService = new UserService(_connectionString);

            var user = await userService.GetByNameAsync(User.Identity.Name);
            if (user == null || user?.Account == null || string.IsNullOrEmpty(user.Account.Name) || string.IsNullOrEmpty(user.Account.Password))
                return Forbid();

            // DOI Check
            var placeholderService = new PlaceholderService(_connectionString);
            var placeholders = await placeholderService.GetByUserIdAsync(user.Id);
            if (!DOIHelper.Validate(model.Data.Attributes.Doi, user.Account.Prefix, user.Pattern, new Dictionary<string, string>(placeholders.Select(p => new KeyValuePair<string, string>(p.Expression, p.RegularExpression)))))
                throw new ForbiddenException($"The doi (doi: {model.Data.Attributes.Doi}) is invalid.");

            //model.Update(_updateProperties);

            var client = new HttpClient
            {
                BaseAddress = new Uri(user.Account.Host),
                DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.Account.Name}:{user.Account.Password}"))) }
            };

            var doiService = new DOIService(_connectionString);
            var doiId = await doiService.CreateAsync(model.Data.Attributes.Doi.GetPrefix(), model.Data.Attributes.Doi.GetSuffix(), (DOIStateType)model.Data.Attributes.Event, user.Id, JsonConvert.SerializeObject(model));

            var response = await client.PostAsync($"dois?publisher=true&affiliation=true", model.AsJson());

            if (!response.IsSuccessStatusCode)
            {
                await doiService.DeleteByIdAsync(doiId);
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            var readDataCiteModel = JsonConvert.DeserializeObject<ReadDataCiteModel>(await response.Content.ReadAsStringAsync());
            await doiService.UpdateByPrefixAndSuffixAsync(model.Data.Attributes.Doi.GetPrefix(), model.Data.Attributes.Doi.GetSuffix(), (DOIStateType)readDataCiteModel!.Data.Attributes.State, await response.Content.ReadAsStringAsync());
            return Created($"{user.Account.Host}/dois/{WebUtility.UrlEncode(model.Data.Attributes.Doi)}", JsonConvert.DeserializeObject<ReadDataCiteModel>(await response.Content.ReadAsStringAsync()));
        }

        /// <summary>
        /// Aktualisiert die Metadaten eines bestehenden DataCite-Eintrags anhand des spezifischen DOI.
        /// </summary>
        /// <param name="doi">Der Digital Object Identifier (DOI) des zu aktualisierenden DataCite-Eintrags.</param>
        /// <param name="model">Das <see cref="UpdateDataCiteModel"/>, das die neuen Daten für die Aktualisierung des DataCite-Eintrags enthält.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem 200 OK-Status, das ein <see cref="ReadDataCiteModel"/>-Objekt zurückgibt,
        /// das die aktualisierten Details des DataCite-Eintrags darstellt.
        /// </returns>
        /// <remarks>
        /// Diese Methode erfordert die Authentifizierung und Autorisierung des Benutzers.
        /// Es wird eine PUT-Anfrage an die externe DOI-API gesendet, um die Metadaten des angegebenen DOI zu aktualisieren.
        /// Die aktualisierten Details werden über den <see cref="DOIService"/> lokal gespeichert.
        /// Der Zugriff auf den externen Dienst erfolgt über gesicherte HTTP-Header mit Hilfe von Basis-Authentifizierung.
        /// Stellen Sie sicher, dass die Account-Daten des Benutzers gültig sind, um die Aktualisierung durchzuführen.
        /// </remarks>
        /// <exception cref="NotFoundException">Wird ausgelöst, wenn das Konto des Benutzers nicht existiert.</exception>
        [HttpPut("datacite/{doi}")]
        [ProducesResponseType(typeof(ReadDataCiteModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PutByDOIAsync(string doi, UpdateDataCiteModel model)
        {
            if (!User.IsInRole("user-datacite") || User?.Identity?.Name == null)
                return Forbid();

            var userService = new UserService(_connectionString);
            var user = await userService.GetByNameAsync(User.Identity!.Name!);

            if (user == null || user?.Account == null || string.IsNullOrEmpty(user.Account.Name) || string.IsNullOrEmpty(user.Account.Password))
                return Forbid();

            var client = new HttpClient
            {
                BaseAddress = new Uri(user.Account.Host),
                DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.Account.Name}:{user.Account.Password}"))) }
            };

            var response = await client.PutAsync($"dois/{doi}?publisher=true&affiliation=true", model.AsJson());

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            var updatedModel = JsonConvert.DeserializeObject<ReadDataCiteModel>(await response.Content.ReadAsStringAsync());

            if (updatedModel == null)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());

            var doiService = new DOIService(_connectionString);
            await doiService.UpdateByDOIAsync(doi, (DOIStateType)updatedModel.Data.Attributes.State, "");

            return Ok(updatedModel);
        }
    }
}