namespace Pakturaly.Infrastructure.Abstractions {
    public interface IQuery<out TResponse> : IRequest<TResponse> { }
}
