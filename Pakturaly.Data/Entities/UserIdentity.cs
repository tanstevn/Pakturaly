using Microsoft.AspNetCore.Identity;
using Pakturaly.Data.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pakturaly.Data.Entities {
    public class UserIdentity : IdentityUser, IUserScoped {
        public long UserId { get; set; }
        [EmailAddress]
        [ProtectedPersonalData]
        public override string? Email { get; set; }
        public string GivenName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        [NotMapped]
        public string FullName => string.Format("{0} {1}", GivenName, LastName);

        public virtual User User { get; set; } = default!;
    }
}
