namespace Pakturaly.Infrastructure.Abstractions {
    public interface ICommand<out TResponse> : IRequest<TResponse> { }
}
