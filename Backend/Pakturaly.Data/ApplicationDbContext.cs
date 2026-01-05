using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pakturaly.Data.Abstractions;
using Pakturaly.Data.Entities;
using System.Diagnostics.CodeAnalysis;

namespace Pakturaly.Data {
    public sealed class ApplicationDbContext : DbContext {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Tenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges() {
            SaveChangesInternally();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
            SaveChangesInternally();
            return base.SaveChangesAsync(cancellationToken);
        }

        [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "")]
        private void SaveChangesInternally() {
            var entries = ChangeTracker.Entries()
                .Where(entry => entry.State == EntityState.Deleted
                    && (entry.Metadata.BaseType is not null
                        ? typeof(ISoftDelete).IsAssignableFrom(entry.Metadata.BaseType.ClrType)
                        : entry.Entity is ISoftDelete)
                    && entry.Metadata.IsOwned()
                    && entry.Metadata is not EntityType {
                        IsImplicitlyCreatedJoinEntityType: true
                    });

            foreach (var scopedEntry in entries) {
                scopedEntry.Property(nameof(ISoftDelete.DeletedAt)).CurrentValue = DateTime.UtcNow;
                scopedEntry.State = EntityState.Modified;
            }
        }

        #region Obsolete Methods
        [Obsolete(@"
        This is to prevent to run multiple times the SaveChangesInternally. 
        Please off-limit yourself from calling this!")]
        public override int SaveChanges(bool acceptAllChangesOnSuccess) {
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        [Obsolete(@"
        This is to prevent to run multiple times the SaveChangesInternally. 
        Please off-limit yourself from calling this!")]
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) {
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        #endregion
    }
}
