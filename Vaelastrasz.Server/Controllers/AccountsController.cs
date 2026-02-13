using LiteDB;
using MethodTimer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaelastrasz.Server.Models;
using Vaelastrasz.Server.Services;

namespace Vaelastrasz.Server.Controllers
{
    [ApiController, Route("api"), Authorize(Roles = "admin"), Time]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<AccountsController> _logger;
        private ConnectionString _connectionString;

        public AccountsController(ILogger<AccountsController> logger, ConnectionString connectionString)
        {
            _logger = logger;
            _connectionString = connectionString;
        }

        /// <summary>
        /// Löscht den Account anhand der angegebenen Id.
        /// </summary>
        /// <param name="id">Die eindeutige Kennung des Accounts, welcher gelöscht werden soll.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/>, das das Ergebnis der Löschoperation repräsentiert.
        /// Der Rückgabewert ist eine HTTP-Antwort mit einem Statuscode, der den Erfolg oder Misserfolg angibt.
        /// Bei Erfolg wird ein 200 OK-Status mit dem Ergebnis der Löschoperation zurückgegeben.
        /// </returns>
        /// <remarks>
        /// Diese Methode verwendet die HttpDelete-Attributroute, um Anfragen unter "/accounts/{id}" zu bearbeiten.
        /// Verwenden Sie diese Methode, um ein Account vollständig aus dem System zu entfernen.
        /// Weitere Informationen finden Sie in der <see href="https://github.com/sventhiel/Vaelastrasz/tree/master/Vaelastrasz.Server#accounts">Dokumentation</see>.
        /// </remarks>
        [HttpDelete("accounts/{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteByIdAsync(long id)
        {
            if (!User.IsInRole("admin"))
                return Forbid();

            var accountService = new AccountService(_connectionString);
            var result = await accountService.DeleteByIdAsync(id);

            return Ok(result);
        }

        /// <summary>
        /// Ruft eine Liste von Accounts ab und gibt sie im JSON-Format zurück.
        /// </summary>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/>, das eine Liste der Accounts als JSON enthält.
        /// Bei Erfolg wird ein 200 OK-Status mit einer Liste von <see cref="ReadAccountModel"/>-Objekten zurückgegeben.
        /// </returns>
        /// <remarks>
        /// Die Methode verwendet den <see cref="AccountService"/>, um die Accounts aus der Datenbank der Anwendung abzurufen.
        /// Jeder Account wird in ein <see cref="ReadAccountModel"/>-Objekt umgewandelt, bevor es an den Client zurückgegeben wird.
        /// Bitte stellen Sie sicher, dass die Verbindungskonfiguration korrekt ist, um Zugriffsprobleme zu vermeiden.
        /// Weitere Informationen finden Sie in der <see href="https://github.com/sventhiel/Vaelastrasz/tree/master/Vaelastrasz.Server#accounts">Dokumentation</see>.
        /// </remarks>
        [HttpGet("accounts")]
        [ProducesResponseType(typeof(List<ReadAccountModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAsync()
        {
            if (!User.IsInRole("admin"))
                return Forbid();

            var accountService = new AccountService(_connectionString);
            var result = await accountService.GetAsync();

            return Ok(new List<ReadAccountModel>(result.Select(a => ReadAccountModel.Convert(a))));
        }

        /// <summary>
        /// Ruft ein Account anhand der angegebenen Id ab und gibt diesen im JSON-Format zurück.
        /// </summary>
        /// <param name="id">Die eindeutige Identifikationsnummer des abzurufenden Accounts.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/>, das den Account im JSON-Format enthält.
        /// Bei Erfolg wird ein 200 OK-Status mit einem <see cref="ReadAccountModel"/>-Objekt zurückgegeben, das die Account-Daten repräsentiert.
        /// Im Falle eines Fehlers, wie z.B. wenn der Account nicht gefunden wird, wird ein entsprechender HTTP-Fehlerstatus zurückgegeben.
        /// </returns>
        /// <remarks>
        /// Diese Methode benutzt den <see cref="AccountService"/>, um einen bestimmten Account basierend auf der übergebenen Id zu finden.
        /// Der Account wird in ein <see cref="ReadAccountModel"/>-Objekt umgewandelt, bevor er an den Client zurückgegeben wird.
        /// Stellen Sie sicher, dass die angegebene Id gültig ist und dass die Datenbankverbindung korrekt konfiguriert ist.
        /// Weitere Informationen finden Sie in der <see href="https://github.com/sventhiel/Vaelastrasz/tree/master/Vaelastrasz.Server#accounts">Dokumentation</see>.
        /// </remarks>
        [HttpGet("accounts/{id}")]
        [ProducesResponseType(typeof(ReadAccountModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByIdAsync(long id)
        {
            if (!User.IsInRole("admin"))
                return Forbid();

            var accountService = new AccountService(_connectionString);
            var result = await accountService.GetByIdAsync(id);

            return Ok(ReadAccountModel.Convert(result));
        }

        /// <summary>
        /// Erstellt einen neuen Account und gibt dessen Details im JSON-Format zurück.
        /// </summary>
        /// <param name="model">Das <see cref="CreateAccountModel"/>, das die Details des zu erstellenden Accounts enthält.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/>, das das Ergebnis der Erstellung zurückgibt.
        /// Bei Erfolg wird ein 201 Created-Status zusammen mit einem <see cref="ReadAccountModel"/>-Objekt zurückgegeben.
        /// Die Antwort enthält auch den URI des neu erstellten Accounts in den HTTP-Headern.
        /// </returns>
        /// <remarks>
        /// Diese Methode erstellt einen neuen Account unter Verwendung der bereitgestellten Modelldaten und gibt die erstellte Ressource zurück.
        /// Bitte stellen Sie sicher, dass alle erforderlichen Felder im <see cref="CreateAccountModel"/> ausgefüllt sind.
        /// Weitere Informationen finden Sie in der <see href="https://github.com/sventhiel/Vaelastrasz/tree/master/Vaelastrasz.Server#accounts">Dokumentation</see>.
        /// </remarks>
        [HttpPost("accounts")]
        [ProducesResponseType(typeof(ReadAccountModel), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PostAsync(CreateAccountModel model)
        {
            if (!User.IsInRole("admin"))
                return Forbid();

            var accountService = new AccountService(_connectionString);

            var id = await accountService.CreateAsync(model.Name, model.Password, model.Host, model.Prefix, model.AccountType);
            var account = await accountService.GetByIdAsync(id);

            var request = HttpContext.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            string resourceUrl = $"{baseUrl}/api/accounts/{account.Id}";

            return Created(resourceUrl, ReadAccountModel.Convert(account));
        }

        /// <summary>
        /// Aktualisiert einen bestehenden Account mit den bereitgestellten Informationen.
        /// </summary>
        /// <param name="id">Die eindeutige Id des zu aktualisierenden Accounts.</param>
        /// <param name="model">Das <see cref="UpdateAccountModel"/>, das die neuen Details für den Account enthält.</param>
        /// <returns>
        /// Ein <see cref="Task{IActionResult}"/>, das den aktualisierten Account im JSON-Format zurückgibt.
        /// Bei Erfolg wird ein 200 OK-Status mit einem <see cref="ReadAccountModel"/>-Objekt zurückgegeben.
        /// </returns>
        /// <remarks>
        /// Diese Methode aktualisiert die Details eines bestehenden Accounts basierend auf der bereitgestellten Id und den neuen Daten im <see cref="UpdateAccountModel"/>.
        /// Bitte stellen Sie sicher, dass die Id eines existierenden Accounts entspricht, um einen erfolgreichen Update-Vorgang zu gewährleisten.
        /// Weitere Informationen finden Sie in der <see href="https://github.com/sventhiel/Vaelastrasz/tree/master/Vaelastrasz.Server#accounts">Dokumentation</see>.
        /// </remarks>
        [HttpPut("accounts/{id}")]
        [ProducesResponseType(typeof(ReadAccountModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PutByIdAsync(long id, UpdateAccountModel model)
        {
            if (!User.IsInRole("admin"))
                return Forbid();

            var accountService = new AccountService(_connectionString);

            var result = await accountService.UpdateByIdAsync(id, model.Name, model.Password, model.Host, model.Prefix);
            var account = await accountService.GetByIdAsync(id);

            return Ok(ReadAccountModel.Convert(account));
        }
    }
}