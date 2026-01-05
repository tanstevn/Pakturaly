using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pakturaly.Data.Extensions;

namespace Pakturaly.Data.Configurations {
    public class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> 
        where TEntity : class {

        public virtual void Configure(EntityTypeBuilder<TEntity> builder) {
            builder
                .ConfigureId()
                .ConfigureSoftDelete();
        }
    }
}
