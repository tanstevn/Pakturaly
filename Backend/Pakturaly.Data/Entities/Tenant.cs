using Pakturaly.Data.Abstractions;

namespace Pakturaly.Data.Entities {
    public class Tenant : ISoftDelete {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<User>? Users { get; set; }
    }
}
