using LiteDB;
using MethodTimer;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Vaelastrasz.Server.Controllers
{
    [ApiController, Route("api"), Time()]
    public class VersionsController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private ConnectionString _connectionString;

        public VersionsController(ILogger<UsersController> logger, ConnectionString connectionString)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        /// <summary>
        /// Ruft die Versionen sowohl der Bibliothek als auch der Anwendung ab und gibt diese im JSON-Format zurück.
        /// </summary>
        /// <returns>
        /// Ein <see cref="IActionResult"/>, das die Version der 'Vaelastrasz.Library' und der aktuellen Anwendung umfasst.
        /// Bei Erfolg wird ein 200 OK-Status mit einem JSON-Objekt zurückgegeben, das die Version der Bibliothek und der Anwendung enthält.
        /// Im Falle eines Fehlers, wie z.B. wenn die Bibliothek nicht gefunden wird, wird ein entsprechender HTTP-Fehlerstatus zurückgegeben.
        /// </returns>
        /// <remarks>
        /// Diese Methode durchläuft die aktuellen Assemblies der Anwendungsdomäne, um die angegebene Bibliothek zu suchen und deren Version zu extrahieren.
        /// Stellen Sie sicher, dass die Bibliothek 'Vaelastrasz.Library' verfügbar ist und korrekt geladen wird, damit die Methode ihre Version abrufen kann.
        /// Weitere Informationen finden Sie in der <see href="https://github.com/sventhiel/Vaelastrasz/tree/master/Vaelastrasz.Server#versions">Dokumentation</see>.
        /// </remarks>
        [HttpGet("versions")]
        public IActionResult GetVersions()
        {
            // Beispiel für Newtonsoft.Json (ersetze dies je nach deinem Paket)
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .SingleOrDefault(a =>
                {
                    // Sicherstellen, dass Name nicht null ist
                    var assemblyName = a.GetName();
                    return assemblyName != null && !string.IsNullOrEmpty(assemblyName.Name) && assemblyName.Name.Equals("Vaelastrasz.Library", StringComparison.OrdinalIgnoreCase);
                });

            if (assembly == null)
                return NotFound("The package 'Vaelastrasz.Library' cannot be found.");

            var version_library = assembly.GetName().Version?.ToString();

            var version_application = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

            return Ok(new
            {
                Library = version_library,
                Application = version_application
            });
        }
    }
}