using Microsoft.EntityFrameworkCore;
using Pakturaly.Data;
using Pakturaly.Infrastructure.Abstractions;
using Pakturaly.Infrastructure.Services;
using Scalar.AspNetCore;
using System.Diagnostics.CodeAnalysis;

namespace Pakturaly.Api {
    public static class DependencyInjection {
        public static void ConfigureConfiguration(IConfigurationManager config) {
            config.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

        public static void ConfigureServices(IServiceCollection services, IConfiguration config) {
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            services.AddOpenApi(options => {
                options.AddDocumentTransformer((document, context, _) => {
                    document.Info = new() {
                        Title = "Pakturaly API",
                        Version = "v1",
                        Description = "API documentation of Pakturaly",
                        Contact = new() {
                            Name = "Pakturaly API Support",
                            Email = "tanstevenlester@gmail.com"
                        }
                    };

                    return Task.CompletedTask;
                });
            });

            services.AddHttpContextAccessor();
            services.AddScoped<ITenantService, TenantService>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("Pakturaly")));
        }

        public static void ConfigureHost(ConfigureHostBuilder host, IConfigurationManager config) {

        }

        [SuppressMessage("Usage", "ASP0014:Suggest using top level route registrations", Justification = "")]
        public static void ConfigureApplication(WebApplication app, IWebHostEnvironment env) {
            if (!env.IsEnvironment("PROD")) {
                app.MapOpenApi().CacheOutput();
                app.MapScalarApiReference();

                app.MapGet("/", () => Results.Redirect("/scalar/v1"))
                    .ExcludeFromDescription();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapFallback(context => {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;

                    return context
                        .Response
                        .WriteAsync(string.Empty);
                });
            });

            // Middlewares here

            InitializeRequiredServices(app);
        }

        private static void InitializeRequiredServices(IApplicationBuilder app) {
            using var scope = app
                .ApplicationServices
                .CreateScope();

            scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>()
                .Database
                .MigrateAsync()
                .GetAwaiter()
                .GetResult();
        }
    }
}
