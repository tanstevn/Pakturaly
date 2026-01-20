using Microsoft.AspNetCore.Mvc;
using Pakturaly.Infrastructure.Abstractions;

namespace Pakturaly.Api.Controllers {
    [Route("api/auth")]
    public class AuthController : BaseController {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) {
            _mediator = mediator;
        }

    }
}
