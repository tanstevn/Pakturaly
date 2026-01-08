using Microsoft.AspNetCore.Identity;
using Pakturaly.Data.Abstractions;

namespace Pakturaly.Data.Entities {
    public class UserRole : IdentityRole<long>, IUserScoped {
        public long? UserId { get; set; }

        public virtual User? User { get; set; }
    }
}
