using FluentValidation;
using Microsoft.AspNetCore.Identity;
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

            RuleFor(param => param.RefreshToken)
                .Must(value => Guid.TryParse(value, out _))
                .When(param => !string.IsNullOrEmpty(param.RefreshToken))
                .WithMessage(param => $"{nameof(param.RefreshToken)} is not a valid GUID format.");
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResult> {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<UserIdentity> _userManager;

        public LoginCommandHandler(ApplicationDbContext dbContext, UserManager<UserIdentity> userManager) {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<LoginCommandResult> HandleAsync(LoginCommand request, CancellationToken cancellationToken = default) {
            var user = await _userManager.FindByEmailAsync(request.Email) 
                ?? throw new Exception(); 
            
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid) {
                throw new Exception();
            }

            using var transaction = await _dbContext.Database
                .BeginTransactionAsync(cancellationToken);

            var refreshToken = new RefreshToken {
                User = user.User,
                Token = Guid.NewGuid()
                    .ToString(),
                ExpiresAt = DateTime.UtcNow
                    .AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            // Generate access token here

            await transaction.CommitAsync(cancellationToken);

            return new LoginCommandResult {
                Id = user.User.Id,
                TenantId = user.User.TenantId.ToString(),
                AccessToken = string.Empty,
                RefreshToken = refreshToken.Token
            };
        }
    }
}
