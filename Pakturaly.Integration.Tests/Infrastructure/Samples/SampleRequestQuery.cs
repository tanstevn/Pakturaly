using Pakturaly.Infrastructure.Abstractions;

namespace Pakturaly.Integration.Tests.Infrastructure.Samples {
    public class SampleRequestQuery : IQuery<SampleRequestQueryResult> { }

    public class SampleRequestQueryResult {
        public bool IsSuccess { get; set; }
    }

    public class SampleRequestQueryHandler : IRequestHandler<SampleRequestQuery, SampleRequestQueryResult> {
        public Task<SampleRequestQueryResult> HandleAsync(SampleRequestQuery request, CancellationToken cancellationToken = default) {
            return Task.FromResult(new SampleRequestQueryResult {
                IsSuccess = true
            });
        }
    }
}
