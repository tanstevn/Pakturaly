using Microsoft.Extensions.Configuration;
using Pakturaly.Infrastructure.Abstractions;

namespace Pakturaly.Application.Auth.Queries {
    public class AuthQuery : IQuery<string> { }

    public class AuthQueryHandler : IRequestHandler<AuthQuery, string> {
        private IConfiguration _config;

        public AuthQueryHandler(IConfiguration config) {
            _config = config;
        }

        public Task<string> HandleAsync(AuthQuery request, CancellationToken cancellationToken = default) {
            return default;
        }
    }
}
