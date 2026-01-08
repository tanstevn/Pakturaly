using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pakturaly.Data.Entities;

namespace Pakturaly.Data.Configurations {
    public class UserCredentialConfiguration : BaseConfiguration<UserCredential> {
        public override void Configure(EntityTypeBuilder<UserCredential> builder) {
            builder.Property(userCred => userCred.Email)
                .HasMaxLength(64)
                .IsRequired();

            builder.HasOne(userCred => userCred.User)
                .WithOne(user => user.Credentials)
                .IsRequired();

            builder.HasIndex(userCred => userCred.UserId);
            builder.HasIndex(userCred => userCred.Email);
            base.Configure(builder);
        }
    }
}
