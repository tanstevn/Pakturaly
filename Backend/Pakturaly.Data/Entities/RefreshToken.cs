using Pakturaly.Data.Abstractions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pakturaly.Data.Entities {
    public class RefreshToken : IUserScoped, ISoftDelete {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Token { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        [NotMapped]
        public bool IsActive => DeletedAt is null && DateTime.UtcNow < ExpiresAt;

        public virtual User User { get; set; } = default!;
    }
}
