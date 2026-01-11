using Microsoft.AspNetCore.Mvc;
using Pakturaly.Application.Auth.Commands;
using Pakturaly.Infrastructure.Abstractions;

namespace Pakturaly.Api.Controllers {
    [Route("api/auth")]
    public class AuthController : BaseController {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command) {
            var result = await _mediator.SendAsync(command);
            return Created($"api/users/{result.Id}", result);
        }

        [/*Authorize,*/ HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command) {
            var result = await _mediator.SendAsync(command);
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshCommand command) {
            var result = await _mediator.SendAsync(command);
            return Ok(result);
        }
    }
}
