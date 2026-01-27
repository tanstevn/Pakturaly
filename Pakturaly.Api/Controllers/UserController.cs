using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pakturaly.Infrastructure.Abstractions;

namespace Pakturaly.Api.Controllers {
    [Authorize, Route("api/users")]
    public class UserController : BaseController {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator) {
            _mediator = mediator;
        }

        [Authorize, HttpGet]
        public IActionResult Example() {
            return Ok("Success!");
        }
    }
}
