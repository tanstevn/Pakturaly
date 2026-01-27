using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pakturaly.Data.Entities;

namespace Pakturaly.Data.Configurations {
    internal sealed class UserConfiguration : BaseConfiguration<User> {

        public override void Configure(EntityTypeBuilder<User> builder) {
            builder.HasOne(user => user.Tenant)
                .WithMany(tenant => tenant.Users)
                .HasForeignKey(user => user.TenantId)
                .IsRequired();

            builder.HasIndex(user => user.TenantId);
            base.Configure(builder);
        }
    }
}
