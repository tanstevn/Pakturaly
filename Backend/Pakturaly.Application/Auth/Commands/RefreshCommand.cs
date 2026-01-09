using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pakturaly.Data;
using Pakturaly.Data.Entities;
using Pakturaly.Infrastructure.Abstractions;

namespace Pakturaly.Application.Auth.Commands {
    public class RefreshCommand : ICommand<RefreshCommandResult> {
        public string RefreshToken { get; set; } = default!;
        public long UserId { get; set; }
    }

    public class RefreshCommandResult {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }

    public class RefreshCommandValidator : AbstractValidator<RefreshCommand> {
        public RefreshCommandValidator() {
            RuleFor(param => param.RefreshToken)
                .Must(value => Guid.TryParse(value, out _))
                .WithMessage(param => $"{nameof(param.RefreshToken)} is not a valid GUID format.");

            RuleFor(param => param.UserId)
                .GreaterThanOrEqualTo(1)
                .WithMessage(param => $"{nameof(param.UserId)} should not be a negative value.");
        }
    }

    public class RefreshCommandHandler : IRequestHandler<RefreshCommand, RefreshCommandResult> {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<UserIdentity> _userManager;

        public RefreshCommandHandler(ApplicationDbContext dbContext, UserManager<UserIdentity> userManager) {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<RefreshCommandResult> HandleAsync(RefreshCommand request, CancellationToken cancellationToken = default) {
            var currentRefreshToken = await _dbContext.RefreshTokens
                .Where(refreshToken => refreshToken.Token == request.RefreshToken)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentRefreshToken is null || !currentRefreshToken.IsActive) {
                throw new Exception(); //
            }

            var user = await _userManager.FindByIdAsync(currentRefreshToken.User.Identity.Email!)
                ?? throw new Exception();

            using var transaction = await _dbContext.Database
                .BeginTransactionAsync(cancellationToken);

            currentRefreshToken.DeletedAt = DateTime.UtcNow;

            var newRefreshToken = new RefreshToken {
                User = user.User,
                Token = Guid.NewGuid()
                    .ToString(),
                ExpiresAt = DateTime.UtcNow
                    .AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            // Generate new access token here

            await transaction.CommitAsync(cancellationToken);

            return new RefreshCommandResult {
                AccessToken = string.Empty,
                RefreshToken = newRefreshToken.Token
            };
        }
    }
}
