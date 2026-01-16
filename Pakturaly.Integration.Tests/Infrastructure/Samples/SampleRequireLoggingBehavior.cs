using Pakturaly.Infrastructure.Abstractions;
using Pakturaly.Shared.Attributes;

namespace Pakturaly.Integration.Tests.Infrastructure.Samples {
    [PipelineOrder(1)]
    public class SampleRequireLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse> {
        public async Task<TResponse> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default) {
            return await next(cancellationToken);
        }
    }
}
