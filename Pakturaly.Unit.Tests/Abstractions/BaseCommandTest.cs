using Pakturaly.Infrastructure.Abstractions;

namespace Pakturaly.Unit.Tests.Abstractions {
    public class BaseCommandTest<TTestClass, TRequest, TResult, TRequestHandler> : BaseUnitTest<TTestClass>, IFluentAssertion<TTestClass, TRequest, TResult, TRequestHandler>
        where TTestClass : BaseCommandTest<TTestClass, TRequest, TResult, TRequestHandler>
        where TRequest : IRequest<TResult>
        where TRequestHandler : class, IRequestHandler<TRequest, TResult> {
        private TRequest _request = default!;
        private TResult _result = default!;
        private Exception _exception = default!;

        public TTestClass Arrange(Action<TRequest> arrange) {
            arrange(_request);
            return (TTestClass)this;
        }

        public TTestClass Act() {
            try {
                _result = default!;
            } catch (Exception ex) {
                _exception = ex;
            }

            return (TTestClass)this;
        }

        public void Assert(Action<TResult> assertion) {
            assertion(_result);
        }

        public void AssertThrows<TException>(Action<TException> assertion)
            where TException : Exception {
            var exception = (TException)_exception;
            assertion(exception);
        }

        public void Dispose() {

        }
    }
}
