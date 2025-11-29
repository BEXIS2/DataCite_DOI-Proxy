using LiteDB;
using MethodTimer;
using Microsoft.AspNetCore.Mvc;
using Vaelastrasz.Library.Extensions;
using Vaelastrasz.Library.Models;

namespace Vaelastrasz.Server.Controllers
{
    [ApiController, Route("api"), Time]
    public class CreateDataCiteModelsController : ControllerBase
    {
        private readonly ILogger<CreateDataCiteModelsController> _logger;
        private ConnectionString _connectionString;

        public CreateDataCiteModelsController(ILogger<CreateDataCiteModelsController> logger, IConfiguration configuration, ConnectionString connectionString)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="property">Derzeit wird 'Creators' und 'Contributors' unterstützt.</param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("createdatacitemodels/prepare/{property}")]
        [ProducesResponseType(typeof(CreateDataCiteModel), 201)]
        public async Task<IActionResult> PostAsync(string property, CreateDataCiteModel model)
        {
            return Ok(model.Update(property));
        }
    }
}