using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Pakturaly.Data;
using Pakturaly.Data.Entities;
using Pakturaly.Infrastructure.Abstractions;
using Pakturaly.Shared.Constants;

namespace Pakturaly.Application.Auth.Commands {
    public class RegisterCommand : ICommand<RegisterCommandResult> {
        public string GivenName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? PhoneNumber { get; set; }
    }

    public class RegisterCommandResult {
        public string Id { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public bool IsSuccess { get; set; }
    }

    public class RegisterCommandValidation : AbstractValidator<RegisterCommand> {
        public RegisterCommandValidation() {
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
            using var transaction = await _dbContext.Database
                .BeginTransactionAsync(cancellationToken);

            var user = new UserIdentity {
                GivenName = request.GivenName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };

            var addUserIdentity = await _userManager.CreateAsync(user, request.Password);

            if (!addUserIdentity.Succeeded) {

            }

            var addUserRole = await _userManager.AddToRoleAsync(user, RoleConstants.Member);

            if (!addUserRole.Succeeded) {

            }

            await transaction.CommitAsync(cancellationToken);
            return new RegisterCommandResult {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                AccessToken = string.Empty,
                RefreshToken = string.Empty,
                IsSuccess = true
            };
        }
    }
}
