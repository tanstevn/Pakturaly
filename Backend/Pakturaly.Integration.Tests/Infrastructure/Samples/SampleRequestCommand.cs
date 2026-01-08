using FluentValidation;
using Pakturaly.Infrastructure.Abstractions;

namespace Pakturaly.Integration.Tests.Infrastructure.Samples {
    public class SampleRequestCommand : ICommand<SampleRequestCommandResult> {
        public long Id { get; set; }
    }

    public class SampleRequestCommandResult {
        public bool IsSuccess { get; set; }
    }

    public class SampleRequestCommandValidator : AbstractValidator<SampleRequestCommand> {
        public SampleRequestCommandValidator() {
            RuleFor(param => param.Id)
                .NotEmpty()
                .WithMessage(param => $"{nameof(param.Id)} should not be empty, null, or 0.")
                .GreaterThanOrEqualTo(1)
                .WithMessage(param => $"{nameof(param.Id)} should not be a negative value.");
        }
    }

    public class SampleRequestCommandHandler : IRequestHandler<SampleRequestCommand, SampleRequestCommandResult> {
        public Task<SampleRequestCommandResult> HandleAsync(SampleRequestCommand request, CancellationToken cancellationToken = default) {
            ArgumentNullException.ThrowIfNull(request);

            return Task.FromResult(new SampleRequestCommandResult {
                IsSuccess = true
            });
        }
    }
}
