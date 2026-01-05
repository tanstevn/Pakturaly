using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pakturaly.Data.Entities;
using Pakturaly.Data.Extensions;

namespace Pakturaly.Data.Configurations {
    public sealed class UserConfiguration : BaseConfiguration<User> {

        public override void Configure(EntityTypeBuilder<User> builder) {
            builder.HasOne(user => user.Tenant)
                .WithMany(tenant => tenant.Users)
                .HasForeignKey(user => user.TenantId)
                .IsRequired();

            builder.ConfigureTenantId();
            base.Configure(builder);
        }
    }
}
