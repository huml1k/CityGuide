using ContentService.Api.Contracts.Responses;
using ContentService.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace ContentService.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);

                await HandleExceptionAsync(context, exception);
            }
        }

        private static async Task HandleExceptionAsync(
     HttpContext context,
     Exception exception)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            var response = new ErrorResponse
            {
                Message = exception.Message,
                TraceId = context.TraceIdentifier
            };

            switch (exception)
            {
                case ValidationException validationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Details = validationException.Errors?
                        .SelectMany(x => x.Value ?? Enumerable.Empty<string>())
                        ?? Enumerable.Empty<string>();
                    break;

                case UnauthorizedException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;

                case ForbiddenException:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    break;

                case NotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                case BusinessRuleException:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "Internal server error.";
                    break;
            }

            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = response.StatusCode;

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
