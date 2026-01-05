using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pakturaly.Api.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase {

        [Authorize]
        [HttpPost("login")]
        public IActionResult Login() {
            return Ok();
        }
    }
}
