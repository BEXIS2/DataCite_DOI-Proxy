using MethodTimer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Vaelastrasz.Library.Models;

namespace Vaelastrasz.Server.Controllers
{
    [ApiController, Route("api"), Time]
    public class ConceptsController : ControllerBase
    {
        private readonly ILogger<ConceptsController> _logger;
        private readonly IWebHostEnvironment _env;

        public ConceptsController(IWebHostEnvironment env, ILogger<ConceptsController> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Ruft das aktuelle DataCite-Metadatenschema als JSON ab.
        /// </summary>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/>, das das aktuelle DataCite-Schema (v4.6) im JSON-Format repräsentiert.
        /// Bei Erfolg wird ein 200 OK-Status mit dem deserialisierten <see cref="ConceptModel"/>-Objekt zurückgegeben.
        /// </returns>
        /// <remarks>
        /// Diese Methode liest die DataCite-Schema-Datei aus dem Verzeichnis "concepts" im Web-Stammverzeichnis der Anwendung.
        /// Die Datei wird mithilfe des Dateipfads "/concepts/datacite.json" geladen.
        /// Das JSON wird in ein <see cref="ConceptModel"/>-Objekt deserialisiert, bevor es an den Client zurückgegeben wird.
        /// Berücksichtigen Sie Sicherheitsaspekte bei der Dateizugriffskonfiguration, um mögliche Pfad- oder Dateizugriffsverletzungen zu vermeiden.
        /// Weitere Informationen finden Sie in der <see href="https://github.com/sventhiel/Vaelastrasz/tree/master/Vaelastrasz.Server#concepts">Dokumentation</see>.
        /// </remarks>
        [HttpGet("concepts")]
        [ProducesResponseType(typeof(ConceptModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync()
        {
            string filePath = Path.Combine(_env.WebRootPath, "concepts", "datacite.json");

            // Lesen Sie den Inhalt der Datei
            string jsonData = await System.IO.File.ReadAllTextAsync(filePath);

            return Ok(JsonConvert.DeserializeObject<ConceptModel>(jsonData));
        }
    }
}