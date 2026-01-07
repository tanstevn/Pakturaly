using Microsoft.AspNetCore.Http;
using Pakturaly.Infrastructure.Abstractions;

namespace Pakturaly.Infrastructure.Services {
    public sealed class TenantService : ITenantService {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantService(IHttpContextAccessor httpContextAccessor) {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid TenantId => _httpContextAccessor.HttpContext?.Items["TenantId"] is Guid tenantId
            ? tenantId
            : Guid.Empty;
    }
}
