namespace Pakturaly.Integration.Tests.Abstractions {
    public interface IIntegrationTest<TTestClass, TRequest, TResult, TRequestHandler> : IDisposable {
        TTestClass Arrange(Action<TRequest> arrange);
        TTestClass Act();
        void Assert(Action<TResult> assertion);
        void AssertThrows<TException>(Action<TException> assertion) where TException : Exception;
    }
}
