using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pakturaly.Application.Extensions;
using Pakturaly.Data;
using Pakturaly.Data.Entities;
using Pakturaly.Infrastructure.Abstractions;

namespace Pakturaly.Application.Auth.Commands {
    public class LoginCommand : ICommand<LoginCommandResult> {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? RefreshToken { get; set; }
    }

    public class LoginCommandResult {
        public long Id { get; set; }
        public string TenantId { get; set; } = default!;
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public int ExpiresIn { get; set; } = default!;
    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand> {
        public LoginCommandValidator() {
            RuleFor(param => param.Email)
                .NotEmpty()
                .WithMessage(param => $"{nameof(param.Email)} should not be empty.")
                .EmailAddress()
                .WithMessage(param => $"{nameof(param.Email)} is not a valid email address.")
                .MaximumLength(64)
                .WithMessage(param => $"{nameof(param.Email)} has a maximum characters limit of 64 only.");

            RuleFor(param => param.Password)
                .NotEmpty()
                .WithMessage(param => $"{nameof(param.Password)} should not be empty.");
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResult> {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<UserIdentity> _userManager;
        private readonly IConfiguration _config;
        private readonly IValidator<LoginCommand> _validator;

        public LoginCommandHandler(ApplicationDbContext dbContext, UserManager<UserIdentity> userManager, 
            IConfiguration config, IValidator<LoginCommand> validator) {
            _dbContext = dbContext;
            _userManager = userManager;
            _config = config;
            _validator = validator;
        }

        public async Task<LoginCommandResult> HandleAsync(LoginCommand request, CancellationToken cancellationToken = default) {
            await _validator.ValidateAndThrowAsync(request, cancellationToken);

            var user = await _dbContext.Users
                .AsTracking()
                .SingleOrDefaultAsync(user => user.Identity.Email == request.Email,
                    cancellationToken);

            if (user is null) {
                throw new UnauthorizedAccessException("User not found.");
            }

            var isPasswordValid = await _userManager
                .CheckPasswordAsync(user.Identity, request.Password);

            if (!isPasswordValid) {
                throw new UnauthorizedAccessException("User not found.");
            }

            var refreshToken = await GetRefreshTokenAsync(request.RefreshToken,
                user, cancellationToken);

            var (accessToken, expiresIn) = user.CreateAccessToken(_config);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new LoginCommandResult {
                Id = user.Id,
                TenantId = user.TenantId
                    .ToString(),
                AccessToken = accessToken,
                ExpiresIn = expiresIn,
                RefreshToken = refreshToken.Token
            };
        }

        private async Task<RefreshToken> GetRefreshTokenAsync(string? refreshToken, User user, CancellationToken cancellationToken) {
            if (!string.IsNullOrEmpty(refreshToken)) {
                var currentRefreshToken = user.RefreshTokens
                    .FirstOrDefault(token => token.Token == refreshToken);

                if (currentRefreshToken is null) {
                    throw new Exception();
                }

                user.RefreshTokens.Remove(currentRefreshToken);
            }

            return user.CreateRefreshToken();
        }
    }
}
