using FluentValidation;
using Pakturaly.Infrastructure.Abstractions;
using Pakturaly.Shared.Attributes;

namespace Pakturaly.Integration.Tests.Infrastructure.Samples {
    [PipelineOrder(2)]
    public class SampleFluentValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
        where TRequest : IRequest<TResponse> {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public SampleFluentValidationBehavior(IEnumerable<IValidator<TRequest>> validators) {
            _validators = validators;
        }

        public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default) {
            var context = new ValidationContext<TRequest>(request);

            var failures = _validators.Select(validator => validator.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(failure => failure is not null)
                .ToList();

            if (failures.Count != default) {
                throw new ValidationException(failures);
            }

            return await next(cancellationToken);    
        }
    }
}
