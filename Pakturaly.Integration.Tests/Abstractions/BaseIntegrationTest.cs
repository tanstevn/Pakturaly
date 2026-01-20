using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pakturaly.Application;
using Pakturaly.Data;
using Pakturaly.Data.Entities;
using Pakturaly.Infrastructure.Abstractions;
using Pakturaly.Infrastructure.Services;

namespace Pakturaly.Integration.Tests.Abstractions {
    public abstract class BaseIntegrationTest<TTestClass, TRequest, TResult, TRequestHandler> : IIntegrationTest<TTestClass, TRequest, TResult, TRequestHandler>
        where TTestClass : BaseIntegrationTest<TTestClass, TRequest, TResult, TRequestHandler>
        where TRequest : IRequest<TResult>
        where TRequestHandler : class, IRequestHandler<TRequest, TResult> {
        private readonly IConfiguration _configuration;
        private readonly ServiceProvider _serviceProvider;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMediator _mediator;

        private TRequest _request = default!;
        private TResult _result = default!;
        private Exception _exception = default!;

        protected BaseIntegrationTest() {
            var services = new ServiceCollection();

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton(_configuration);

            services.AddHttpContextAccessor();
            services.AddScoped<ITenantService, TenantService>();

            ConfigureMediator(services);
            ConfigureDatabase(services);

            _serviceProvider = services.BuildServiceProvider();
            _dbContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            _mediator = _serviceProvider.GetRequiredService<IMediator>();
        }

        private void ConfigureMediator(IServiceCollection services) {
            services.AddScoped<IMediator, MediatorService>();
            services.AddValidatorsFromAssembly(typeof(MediatorAnchor).Assembly);

            //services.AddTransient<IPipelineBehavior<TRequest, TResult>, ExampleLoggingBehavior<TRequest, TResult>>();
            //services.AddTransient<IPipelineBehavior<TRequest, TResult>, ExampleValidationBehavior<TRequest, TResult>>();

            services.AddTransient<IRequestHandler<TRequest, TResult>, TRequestHandler>();
        }

        private void ConfigureDatabase(IServiceCollection services) {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("Pakturaly")));

            services
                .AddIdentityCore<UserIdentity>(options => {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequiredUniqueChars = 1;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>();
        }

        public TTestClass Arrange(Action<TRequest> arrange) {
            _request = Activator.CreateInstance<TRequest>();
            arrange(_request);

            return (TTestClass)this;
        }

        public TTestClass Act() {
            try {
                _result = _mediator.SendAsync(_request)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception ex) {
                _exception = ex;
            }

            return (TTestClass)this;
        }

        public void Assert(Action<TResult> assertion) {
            assertion(_result);
        }

        public void AssertThrows<TException>(Action<TException> assertion)
            where TException : Exception {
            ArgumentNullException.ThrowIfNull(_exception);

            _exception.Should()
                .BeOfType<TException>();

            assertion((TException)_exception);
        }

        public void Dispose() {
            _dbContext.Dispose();
            _serviceProvider.Dispose();
        }
    }
}
