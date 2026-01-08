using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pakturaly.Data.Entities;

namespace Pakturaly.Data.Configurations {
    public class UserRoleConfiguration : BaseConfiguration<UserRole> {
        public override void Configure(EntityTypeBuilder<UserRole> builder) {
            builder.HasOne(userRole => userRole.User)
                .WithMany(user => user.Roles)
                .HasForeignKey(userRole => userRole.UserId)
                .IsRequired();

            builder.HasIndex(userRole => userRole.UserId);
            base.Configure(builder);
        }
    }
}
