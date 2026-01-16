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
    }

    public class RefreshCommandResult {
        public string AccessToken { get; set; } = default!;
        public int ExpiresIn { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }

    public class RefreshCommandValidator : AbstractValidator<RefreshCommand> {
        public RefreshCommandValidator() {
            RuleFor(param => param.RefreshToken)
                .NotEmpty()
                .WithMessage(param => $"{nameof(param.RefreshToken)} should not be empty.");
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
                .AsTracking()
                .FirstOrDefaultAsync(refreshToken => refreshToken.Token == request.RefreshToken,
                    cancellationToken);

            if (currentRefreshToken is null) {
                throw new Exception(); //
            }

            var user = currentRefreshToken.User;
            var (accessToken, expiresIn) = user.CreateAccessToken(_config);

            var newRefreshToken = user.CreateRefreshToken();
            user.RefreshTokens.Remove(currentRefreshToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new RefreshCommandResult {
                AccessToken = accessToken,
                ExpiresIn = expiresIn,
                RefreshToken = newRefreshToken.Token
            };
        }
    }
}
