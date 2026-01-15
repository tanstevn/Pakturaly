using Microsoft.AspNetCore.Diagnostics;

namespace Pakturaly.Api.Middlewares {
    public class GlobalExceptionHandler : IExceptionHandler {
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger) {
            _problemDetailsService = problemDetailsService;
            _logger = logger;
        }

        public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
    }
}
