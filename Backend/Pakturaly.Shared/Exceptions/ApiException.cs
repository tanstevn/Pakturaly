namespace Pakturaly.Shared.Exceptions {
    public class ApiException : Exception {
        public ApiException(string? message = default) : base(message) { }
    }
}
