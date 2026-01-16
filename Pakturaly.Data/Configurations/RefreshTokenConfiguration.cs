using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pakturaly.Data.Entities;

namespace Pakturaly.Data.Configurations {
    public class RefreshTokenConfiguration : BaseConfiguration<RefreshToken> {
        public override void Configure(EntityTypeBuilder<RefreshToken> builder) {
            builder.Property(refreshToken => refreshToken.Token)
                .IsRequired();

            builder.Property(refreshToken => refreshToken.ExpiresAt)
                .IsRequired();

            builder.Property(refreshToken => refreshToken.CreatedAt)
                .IsRequired();

            builder.HasOne(refreshToken => refreshToken.User)
                .WithMany(user => user.RefreshTokens);

            builder.Navigation(refreshToken => refreshToken.User)
                .AutoInclude();

            builder.HasIndex(refreshToken => refreshToken.Token);
            base.Configure(builder);
        }
    }
}
