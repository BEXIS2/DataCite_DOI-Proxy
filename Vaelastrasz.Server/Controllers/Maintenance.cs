using MethodTimer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Vaelastrasz.Server.Controllers
{
    [ApiController, Route(""), Time]
    public class Maintenance : ControllerBase
    {
        [HttpHead]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Head() => Ok(new { status = "OK", timestamp = DateTime.UtcNow });
    }
}
