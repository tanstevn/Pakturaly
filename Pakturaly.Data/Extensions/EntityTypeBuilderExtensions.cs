using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pakturaly.Data.Abstractions;

namespace Pakturaly.Data.Extensions {
    public static class EntityTypeBuilderExtensions {
        public static EntityTypeBuilder<TEntity> ConfigureId<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class {
            builder.HasKey("Id");

            var idProp = builder.Property("Id");

            if (idProp.Metadata.ClrType != typeof(Guid)) {
                idProp.ValueGeneratedOnAdd();
            }
            else {
                idProp.HasDefaultValueSql("NEWSEQUENTIALID()")
                    .ValueGeneratedOnAdd();
            }

            return builder;
        }

        public static EntityTypeBuilder<TEntity> ConfigureSoftDelete<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class {
            if (!typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity))) {
                return builder;
            }

            builder.HasQueryFilter(entity => !((ISoftDelete)entity).DeletedAt.HasValue);
            return builder;
        }

        public static EntityTypeBuilder<TEntity> ConfigureTenantUserIndex<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class {
            if (!typeof(ITenantUserScoped).IsAssignableFrom(typeof(TEntity))) {
                return builder;
            }

            builder.HasIndex(e => new { ((ITenantUserScoped)e).TenantId, ((ITenantUserScoped)e).UserId });
            return builder;
        }
    }
}
