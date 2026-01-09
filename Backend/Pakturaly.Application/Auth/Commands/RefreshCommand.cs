using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pakturaly.Application.Extensions;
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
        private readonly IConfiguration _config;

        public RefreshCommandHandler(ApplicationDbContext dbContext, UserManager<UserIdentity> userManager, IConfiguration config) {
            _dbContext = dbContext;
            _userManager = userManager;
            _config = config;
        }

        public async Task<RefreshCommandResult> HandleAsync(RefreshCommand request, CancellationToken cancellationToken = default) {
            var currentRefreshToken = await _dbContext.RefreshTokens
                .Where(refreshToken => refreshToken.Token == request.RefreshToken)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentRefreshToken is null || !currentRefreshToken.IsActive) {
                throw new Exception(); //
            }

            var user = await _userManager.FindByIdAsync(currentRefreshToken.User.Identity.Email!)
                ?? throw new Exception(); //

            using var transaction = await _dbContext.Database
                .BeginTransactionAsync(cancellationToken);

            currentRefreshToken.DeletedAt = DateTime.UtcNow;

            var userRoles = await _userManager.GetRolesAsync(user);
            var accessToken = user.GenerateAccessToken(userRoles, _config);
            var newRefreshToken = user.GenerateRefreshToken();

            await transaction.CommitAsync(cancellationToken);

            return new RefreshCommandResult {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token
            };
        }
    }
}
