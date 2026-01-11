using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pakturaly.Application.Extensions;
using Pakturaly.Data;
using Pakturaly.Data.Entities;
using Pakturaly.Infrastructure.Abstractions;

namespace Pakturaly.Application.Auth.Commands {
    public class RegisterCommand : ICommand<RegisterCommandResult> {
        public string GivenName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        public string? TenantId { get; set; }
    }

    public class RegisterCommandResult {
        public long Id { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public bool IsSuccess { get; set; }
    }

    public class RegisterCommandValidator : AbstractValidator<RegisterCommand> {
        public RegisterCommandValidator() {
            RuleFor(param => param.GivenName)
                .NotEmpty()
                .WithMessage(param => $"{nameof(param.GivenName)} should not be empty.")
                .MaximumLength(64)
                .WithMessage(param => $"{nameof(param.GivenName)} has a maximum characters limit of 64 only.");

            RuleFor(param => param.LastName)
                .NotEmpty()
                .WithMessage(param => $"{nameof(param.LastName)} should not be empty.")
                .MaximumLength(64)
                .WithMessage(param => $"{nameof(param.LastName)} has a maximum characters limit of 64 only.");

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

            RuleFor(param => param.TenantId)
                .Must(value => Guid.TryParse(value, out _))
                .When(param => !string.IsNullOrEmpty(param.TenantId))
                .WithMessage(param => $"{nameof(param.TenantId)} is not a valid GUID format.");
        }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterCommandResult> {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<UserIdentity> _userManager;

        public RegisterCommandHandler(ApplicationDbContext dbContext, UserManager<UserIdentity> userManager) {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<RegisterCommandResult> HandleAsync(RegisterCommand request, CancellationToken cancellationToken = default) {
            var utcNow = DateTime.UtcNow;

            using var transaction = await _dbContext.Database
                .BeginTransactionAsync(cancellationToken);

            var tenant = await GetTenantAsync(request.TenantId, 
                request.GivenName, request.LastName, cancellationToken);

            var user = new User {
                CreatedAt = utcNow,
                Tenant = tenant!
            };

            var refreshToken = user.CreateRefreshToken();

            var userIdentity = new UserIdentity {
                GivenName = request.GivenName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
                User = user
            };

            var addUserIdentity = await _userManager.CreateAsync(userIdentity, request.Password);

            if (!addUserIdentity.Succeeded) {
                await transaction.RollbackAsync(cancellationToken);
                throw new Exception(); //
            }

            await transaction.CommitAsync(cancellationToken);

            return new RegisterCommandResult {
                Id = user.Id,
                FullName = user.Identity.FullName,
                Email = user.Identity.Email!,
                RefreshToken = refreshToken.Token,
                IsSuccess = true
            };
        }

        private async Task<Tenant> GetTenantAsync(string? tenantId, string givenName, string lastName, CancellationToken cancellationToken) {
            if (!string.IsNullOrEmpty(tenantId)
                && Guid.TryParse(tenantId, out var parsedTenantId)) {
                var tenant = await _dbContext.Tenants
                    .FirstOrDefaultAsync(tenant => tenant.Id == parsedTenantId, cancellationToken);

                if (tenant is not null) {
                    return tenant;
                }
            }

            return new Tenant {
                Id = Guid.NewGuid(),
                Name = string.Format("{0} {1}", givenName, lastName),
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
