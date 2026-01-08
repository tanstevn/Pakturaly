using Microsoft.AspNetCore.Identity;
using Pakturaly.Data.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Pakturaly.Data.Entities {
    public class UserIdentity : IdentityUser, IUserScoped {
        public long UserId { get; set; }
        [EmailAddress]
        [ProtectedPersonalData]
        public override string? Email { get; set; }
        public string GivenName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? FullName { get; set; }

        public virtual User User { get; set; } = default!;
    }
}
