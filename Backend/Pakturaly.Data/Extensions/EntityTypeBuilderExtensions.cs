using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pakturaly.Data.Abstractions;

namespace Pakturaly.Data.Extensions {
    public static class EntityTypeBuilderExtensions {

        public static EntityTypeBuilder<TEntity> ConfigureId<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class {
            builder.HasKey("Id");

            builder.Property("Id")
                .ValueGeneratedOnAdd();

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

        public static EntityTypeBuilder<TEntity> ConfigureTenantId<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class {
            if (!typeof(ITenantScoped).IsAssignableFrom(typeof(TEntity))) {
                return builder;
            }

            builder.HasIndex(entity => ((ITenantScoped)entity).TenantId);
            return builder;
        }

        public static EntityTypeBuilder<TEntity> ConfigureUserId<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class {
            if (!typeof(IUserScoped).IsAssignableFrom(typeof(TEntity))) {
                return builder;
            }

            builder.HasIndex(entity => ((IUserScoped)entity).UserId);
            return builder;
        }

        public static EntityTypeBuilder<TEntity> ConfigureTenantAndUserIds<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class {
            if (!typeof(ITenantUserScoped).IsAssignableFrom(typeof(TEntity))) {
                return builder;
            }

            builder.HasIndex(e => new { ((ITenantUserScoped)e).TenantId, ((ITenantUserScoped)e).UserId });
            return builder;
        }
    }
}
