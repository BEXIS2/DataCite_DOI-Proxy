using MethodTimer;
using Microsoft.AspNetCore.Mvc;
using NameParser;

namespace Vaelastrasz.Server.Controllers
{
    [ApiController, Route("api"), Time]
    public class NamesController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;

        public NamesController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Verarbeitet einen übergebenen Namen und gibt ihn als <see cref="HumanName"/>-Objekt zurück.
        /// </summary>
        /// <param name="name">Der Name, der verarbeitet werden soll.</param>
        /// <returns>
        /// Ein <see cref="IActionResult"/> mit einem 200 OK-Status und einem <see cref="HumanName"/>-Objekt bei erfolgreicher Verarbeitung,
        /// oder ein 400 Bad Request-Status, wenn der Name ungültig ist oder nicht geparst werden kann.
        /// </returns>
        /// <remarks>
        /// Diese Methode überprüft, ob der übergebene Name gültig und parsbar ist.
        /// Ist der Name null oder leer, oder kann er nicht in ein <see cref="HumanName"/>-Objekt umgewandelt werden, wird eine entsprechende
        /// HTTP-Fehlermeldung zurückgegeben. Der Parsing-Prozess erfolgt durch die Instanz der <see cref="HumanName"/>-Klasse.
        /// Entwickler sollten sicherstellen, dass die Eingabe nichtnull und korrekt formatiert ist, um eine erfolgreiche Verarbeitung
        /// sicherzustellen.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Wird ausgelöst, wenn der übergebene Name null ist.</exception>
        [HttpPost("names")]
        [ProducesResponseType(typeof(HumanName), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
        public IActionResult Post([FromBody] string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest();

            var humanName = new HumanName(name);

            if (humanName.IsUnparsable)
                return BadRequest();

            return Ok(humanName);
        }
    }
}