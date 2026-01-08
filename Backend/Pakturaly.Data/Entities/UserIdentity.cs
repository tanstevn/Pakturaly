using Microsoft.AspNetCore.Identity;
using Pakturaly.Data.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Pakturaly.Data.Entities {
    public class UserIdentity : IdentityUser, IUserScoped {
        public long? UserId { get; set; }
        [EmailAddress]
        [ProtectedPersonalData]
        public override string? Email { get; set; }
        public required string GivenName { get; set; }
        public required string LastName { get; set; }
        public string? FullName { get; set; }

        public virtual User? User { get; set; }
    }
}
