using Pakturaly.Infrastructure.Abstractions;

namespace Pakturaly.Application.Auth.Commands {
    public class ExampleCommand : ICommand<string> {
        public long Id { get; set; }
    }
}
