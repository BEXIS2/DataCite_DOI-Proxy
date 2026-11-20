using LiteDB;
using MethodTimer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vaelastrasz.Server.Models;
using Vaelastrasz.Server.Services;

namespace Vaelastrasz.Server.Controllers
{
    [ApiController, Route("api"), Time]
    public class ORCiDController : ControllerBase
    {
        private readonly ILogger<ORCiDController> _logger;
        private ConnectionString _connectionString;

        public ORCiDController(ILogger<ORCiDController> logger, ConnectionString connectionString)
        {
            _logger = logger;
            _connectionString = connectionString;
        }

        [HttpGet("orcid/{id}")]
        [ProducesResponseType(typeof(ReadAccountModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            if (!User.IsInRole("admin"))
                return Forbid();

            var accountService = new AccountService(_connectionString);
            var result = await accountService.GetByIdAsync(id);

            return Ok(ReadAccountModel.Convert(result));
        }
    }
}
