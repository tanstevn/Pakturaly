using Microsoft.AspNetCore.Mvc;

namespace Pakturaly.Api.Controllers {
    [ApiController, Produces("application/json")]
    public class BaseController : ControllerBase {

        [HttpGet("api/test")]
        public IActionResult Test() {
            return Ok("API is working!");
        }
    }
}
