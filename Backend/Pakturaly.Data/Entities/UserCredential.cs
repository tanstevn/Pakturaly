using Microsoft.AspNetCore.Identity;
using Pakturaly.Data.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Pakturaly.Data.Entities {
    public class UserCredential : IdentityUser<long>, IUserScoped {
        public long? UserId { get; set; }
        [EmailAddress]
        [ProtectedPersonalData]
        public override string? Email { get; set; }

        public virtual User? User { get; set; }
    }
}
