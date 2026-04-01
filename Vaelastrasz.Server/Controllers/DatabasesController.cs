using LiteDB;
using MethodTimer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaelastrasz.Library.Exceptions;
using Vaelastrasz.Library.Extensions;
using Vaelastrasz.Library.Models;

namespace Vaelastrasz.Server.Controllers
{
    [ApiController, Route("api"), Authorize(Roles = "admin"), Time]
    [RequestSizeLimit(32000000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 32000000)]
    public class DatabasesController : ControllerBase
    {
        private readonly ILogger<AccountsController> _logger;
        private ConnectionString _connectionString;

        public DatabasesController(ILogger<AccountsController> logger, ConnectionString connectionString)
        {
            _logger = logger;
            _connectionString = connectionString;
        }

        /// <summary>
        /// Löscht die Datenbankdatei, die über den aktuellen Verbindungsstring konfiguriert ist.
        /// </summary>
        /// <returns>
        /// Ein <see cref="IActionResult"/> mit einem 200 OK, wenn die Datenbankdatei erfolgreich gelöscht wurde.
        /// </returns>
        /// <remarks>
        /// Diese Methode berücksichtigt die aktuelle Konfiguration des Verbindungsstrings, um die entsprechende Datenbankdatei zu lokalisieren und zu löschen.
        /// Stellen Sie sicher, dass keine aktiven Verbindungen zur Datenbank bestehen, um Probleme beim Löschen zu vermeiden.
        /// Diese Methode sollte mit Vorsicht verwendet werden, da der Löschvorgang nicht rückgängig gemacht werden kann.
        /// Die erfolgreiche Ausführung erfordert ausreichende Berechtigungen für den Zugriff auf das Dateisystem.
        /// </remarks>
        [HttpDelete("databases")]
        [ProducesResponseType(typeof(ConceptModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult DeleteAsync()
        {
            if (!User.IsInRole("admin"))
                return Forbid();

            FileInfo database = new FileInfo(_connectionString.Filename);

            database.Delete();

            if (database.Exists)
                return NotFound();

            return Ok(true);
        }

        /// <summary>
        /// Bietet die aktuelle Datenbankdatei als Download an.
        /// </summary>
        /// <returns>
        /// Ein <see cref="IActionResult"/>, das die Datenbankdatei als "application/octet-stream" zum Download bereitstellt.
        /// Bei Erfolg wird die Datei mit einem dynamischen Dateinamen, der das Abrufdatum und die -zeit enthält, zurückgegeben.
        /// </returns>
        /// <remarks>
        /// Diese Methode stellt die Datenbankdatei, die durch den aktuellen Verbindungsstring beschrieben wird, als Datei-Download bereit.
        /// Der Dateiname der bereitgestellten Datei enthält das ursprüngliche Datenbankpräfix und das aktuelle Datum und die aktuelle Uhrzeit, um die Versionskontrolle zu erleichtern.
        /// Es ist wichtig, dass die Zugriffsberechtigungen korrekt gesetzt sind, um sicherzustellen, dass der Dateidownload funktioniert.
        /// </remarks>
        [HttpGet("databases")]
        [ProducesResponseType(typeof(ConceptModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public IActionResult GetAsync()
        {
            if (!User.IsInRole("admin"))
                return Forbid();

            // database
            var databasePath = _connectionString.Filename;
            var fileInfo = new FileInfo(databasePath);
            if (!fileInfo.Exists)
                return NotFound();

            var downloadFileName = $"{Path.GetFileNameWithoutExtension(fileInfo.Name)}_{DateTimeOffset.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(fileInfo.Name)}";

            var fileStream = System.IO.File.OpenRead(databasePath);
            return File(fileStream, "application/octet-stream", downloadFileName);
        }

        /// <summary>
        /// Speichert eine hochgeladene Datenbankdatei auf dem Server.
        /// </summary>
        /// <param name="file">Die hochgeladene Datei, die als neue Datenbank verwendet werden soll.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/> mit einem 201 Created-Status, wenn die Datei erfolgreich gespeichert wurde.
        /// </returns>
        /// <remarks>
        /// Diese Methode überprüft das hochgeladene Dateiobjekt auf Gültigkeit, einschließlich Dateityp, Dateigröße und MIME-Typ.
        /// Nur Dateien mit der Erweiterung ".db" und bestimmten unterstützten MIME-Typen werden akzeptiert.
        /// Die Datei wird an dem Ort gespeichert, der im Verbindungsstring als Datenbank definiert ist.
        /// Stellen Sie sicher, dass die hochgeladene Datei den Anforderungen entspricht und ausreichende Berechtigungen zum Speichern der Datei vorhanden sind.
        /// </remarks>
        /// <exception cref="BadRequestException">Wird ausgelöst, wenn die hochgeladene Datei nicht den Anforderungen entspricht oder ungültig ist.</exception>
        [HttpPost("databases")]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostAsync(IFormFile file)
        {
            if (!User.IsInRole("admin"))
                return Forbid();

            if (file == null)
                //throw new BadRequestException("null");
                return BadRequest("null");

            if (file.Length == 0)
                //throw new BadRequestException("0");
                return BadRequest("0");

            // Validate file extension
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension))
                //throw new BadRequestException("empty extension");
                return BadRequest("empty extension");

            if (extension != ".db")
                //throw new BadRequestException("!= .db extension");
                return BadRequest("!= .db extension");

            var mimeTypes = new List<string> { "application/x-litedb", "application/octet-stream" };
            if (!mimeTypes.Contains(file.ContentType))
                //throw new BadRequestException("mime type");
                return BadRequest("mime type");

            string databasePath = new FileInfo(_connectionString.Filename).FullName;

            // Save the file to the server
            using (var stream = new FileStream(databasePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Created();
        }
    }
}