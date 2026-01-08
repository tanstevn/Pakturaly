using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Pakturaly.Infrastructure.Abstractions;
using Pakturaly.Infrastructure.Services;
using Pakturaly.Integration.Tests.Infrastructure.Samples;

namespace Pakturaly.Integration.Tests.Infrastructure.Services {
    public class MediatorServiceTests {
        private readonly IServiceCollection _services;
        private readonly IMediator _mediator;

        public MediatorServiceTests() {
            _services = new ServiceCollection();
            _services.AddScoped<IMediator, MediatorService>();
            _services.AddValidatorsFromAssembly(typeof(SampleMediatorAnchor).Assembly);

            #region Pipeline Behaviors
            _services.AddTransient<IPipelineBehavior<SampleRequestQuery, SampleRequestQueryResult>, SampleFluentValidationBehavior<SampleRequestQuery, SampleRequestQueryResult>>();
            _services.AddTransient<IPipelineBehavior<SampleRequestQuery, SampleRequestQueryResult>, SampleRequireLoggingBehavior<SampleRequestQuery, SampleRequestQueryResult>>();

            _services.AddTransient<IPipelineBehavior<SampleRequestCommand, SampleRequestCommandResult>, SampleFluentValidationBehavior<SampleRequestCommand, SampleRequestCommandResult>>();
            _services.AddTransient<IPipelineBehavior<SampleRequestCommand, SampleRequestCommandResult>, SampleFluentValidationBehavior<SampleRequestCommand, SampleRequestCommandResult>>();
            #endregion

            #region Request Handlers
            _services.AddTransient<IRequestHandler<SampleRequestQuery, SampleRequestQueryResult>, SampleRequestQueryHandler>();
            _services.AddTransient<IRequestHandler<SampleRequestCommand, SampleRequestCommandResult>, SampleRequestCommandHandler>();
            #endregion

            _mediator = _services
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();
        }

        [Fact, Trait("Category", "Integration")]
        public async Task Mediator_Request_Query_Runs_Successfully() {
            // Arrange
            var request = new SampleRequestQuery();

            // Act
            var result = await _mediator.SendAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact, Trait("Category", "Integration")]
        public async Task Mediator_Request_Command_Runs_Successfully() {
            // Arrange
            var request = new SampleRequestCommand {
                Id = long.MaxValue
            };

            // Act
            var result = await _mediator.SendAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact, Trait("Category", "Integration")]
        public async Task Mediator_Request_Command_Failed_Validation_With_Negative_Value_Throws_Exception() {
            // Arrange
            var request = new SampleRequestCommand {
                Id = long.MinValue
            };

            // Act
            var result = _mediator.SendAsync(request);

            // Assert
            var exception = await Assert
                .ThrowsAsync<ValidationException>(async () => await result);

            Assert.True(exception is not null and ValidationException);
            Assert.NotEmpty(exception.Errors);
            Assert.Contains("should not be a negative value", exception.Message);
        }

        [Fact, Trait("Category", "Integration")]
        public async Task Mediator_Request_Command_Failed_Validation_With_Default_Value_Throws_Exception() {
            // Arrange
            var request = new SampleRequestCommand {
                Id = default
            };

            // Act
            var result = _mediator.SendAsync(request);

            // Assert
            var exception = await Assert
                .ThrowsAsync<ValidationException>(async () => await result);

            Assert.True(exception is not null and ValidationException);
            Assert.NotEmpty(exception.Errors);
            Assert.Contains("should not be empty, null, or 0", exception.Message);
        }
    }
}
