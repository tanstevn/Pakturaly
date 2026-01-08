using Pakturaly.Data.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Pakturaly.Data.Entities {
    public class UserDetail : IUserScoped {
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public string? GivenName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }

        public virtual User? User { get; set; }
    }
}
