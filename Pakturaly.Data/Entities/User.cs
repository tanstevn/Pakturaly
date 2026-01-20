using Pakturaly.Data.Abstractions;

namespace Pakturaly.Data.Entities {
    public class User : ITenantScoped, ISoftDelete {
        public long Id { get; set; }
        public Guid TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Tenant Tenant { get; set; } = default!;
    }
}
