using Microsoft.Extensions.DependencyInjection;
using Pakturaly.Infrastructure.Abstractions;
using Pakturaly.Shared.Attributes;
using System.Reflection;

namespace Pakturaly.Infrastructure.Services {
    internal sealed class MediatorExecutorService<TRequest, TResponse> : IMediatorExecutor where TRequest : IRequest<TResponse> {
        public async Task<object> ExecuteAsync(object request, IServiceProvider serviceProvider, CancellationToken cancellationToken = default) {
            var requestHandler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

            ArgumentNullException.ThrowIfNull(requestHandler);

            RequestHandlerDelegate<TResponse> lastHandler = (token)
                => requestHandler.HandleAsync((TRequest)request, token);

            var pipelineBehaviors = serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>()
                ?? [];

            var behaviorOrder = pipelineBehaviors
                .OrderByDescending(behavior => behavior.GetType()
                    .GetCustomAttribute<PipelineOrderAttribute>()!
                    .Order);

            var aggregateResult = behaviorOrder.Aggregate(lastHandler, (next, behavior) => (token)
                    => behavior.HandleAsync((TRequest)request, next, token));

            return (await aggregateResult(cancellationToken))!;
        }
    }
}
