using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pakturaly.Data.Entities;

namespace Pakturaly.Data.Configurations {
    public class UserDetailConfiguration : BaseConfiguration<UserDetail> {
        public override void Configure(EntityTypeBuilder<UserDetail> builder) {
            builder.Property(userDetail => userDetail.GivenName)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(userDetail => userDetail.LastName)
                .HasMaxLength(64)
                .IsRequired();

            builder.HasOne(userDetail => userDetail.User)
                .WithOne(user => user.Details);
            
            builder.HasIndex(user => user.UserId)
                .IsUnique();

            base.Configure(builder);
        }
    }
}
