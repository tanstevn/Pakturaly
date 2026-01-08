using Pakturaly.Infrastructure.Abstractions;
using System.Collections.Concurrent;

namespace Pakturaly.Infrastructure.Services {
    public sealed class MediatorService : IMediator {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<Type, IMediatorExecutor> _requestHandlers;
        
        public MediatorService(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
            _requestHandlers = new();
        }

        public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) {
            ArgumentNullException.ThrowIfNull(request);

            var requestHandler = _requestHandlers
                .GetOrAdd(request.GetType(), requestType => {
                    var executorType = typeof(MediatorExecutorService<,>)
                        .MakeGenericType(requestType, typeof(TResponse));

                    return (IMediatorExecutor)Activator.CreateInstance(executorType)!;
                });

            var result = await requestHandler.ExecuteAsync(requestHandler, _serviceProvider, cancellationToken);
            return (TResponse)result;
        }
    }
}
