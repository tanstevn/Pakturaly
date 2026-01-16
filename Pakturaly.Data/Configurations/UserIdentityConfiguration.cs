using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pakturaly.Data.Entities;

namespace Pakturaly.Data.Configurations {
    public class UserIdentityConfiguration : BaseConfiguration<UserIdentity> {
        public override void Configure(EntityTypeBuilder<UserIdentity> builder) {
            builder.Property(userIdentity => userIdentity.Email)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(userIdentity => userIdentity.GivenName)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(userIdentity => userIdentity.LastName)
                .HasMaxLength(64)
                .IsRequired();

            builder.HasOne(userIdentity => userIdentity.User)
                .WithOne(user => user.Identity);

            builder.HasIndex(userIdentity => userIdentity.UserId);
            builder.HasIndex(userIdentity => userIdentity.Email);
            base.Configure(builder);
        }
    }
}
