namespace Pakturaly.Infrastructure.Abstractions {
    public interface IMediatorExecutor {
        Task<object> ExecuteAsync(object request, IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
    }
}
