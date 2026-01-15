using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pakturaly.Shared.Utils;
using System.Text.Json;

namespace Pakturaly.Api.Middlewares {
    public class ValidationExceptionHandler : IExceptionHandler {
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<ValidationExceptionHandler> _logger;

        public ValidationExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<ValidationExceptionHandler> logger) {
            _problemDetailsService = problemDetailsService;
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken) {
            if (exception is not ValidationException validationEx) {
                return false;
            }

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            var problemContext = new ProblemDetailsContext {
                HttpContext = httpContext,
                ProblemDetails = new ProblemDetails {
                    Detail = "One or more errors occured.",
                    Status = StatusCodes.Status400BadRequest
                }
            };

            var errors = validationEx.Errors
                .GroupBy(err => err.PropertyName)
                .ToDictionary(
                    property => property.Key
                        .ToCamelCase(),
                    failures => failures.Select(err => err.ErrorMessage)
                        .ToList());

            problemContext.ProblemDetails.Extensions.Add("errors", errors);

            return await _problemDetailsService
                .TryWriteAsync(problemContext);
        }
    }
}
