using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pakturaly.Data.Entities;

namespace Pakturaly.Data.Configurations {
    public sealed class TenantConfiguration : BaseConfiguration<Tenant> {
        
        public override void Configure(EntityTypeBuilder<Tenant> builder) {
            builder.HasMany(tenant => tenant.Users)
                .WithOne(user => user.Tenant);

            base.Configure(builder);
        }
    }
}
