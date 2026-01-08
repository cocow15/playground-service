using System.Diagnostics;

namespace AuthService.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        try
        {
            // Log incoming request
            _logger.LogInformation(
                "Incoming {Method} request to {Path} from {ClientIp}",
                requestMethod,
                requestPath,
                clientIp
            );

            await _next(context);

            stopwatch.Stop();

            // Log response
            _logger.LogInformation(
                "Completed {Method} {Path} with status {StatusCode} in {ElapsedMs}ms from {ClientIp}",
                requestMethod,
                requestPath,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                clientIp
            );
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "Request {Method} {Path} from {ClientIp} failed after {ElapsedMs}ms",
                requestMethod,
                requestPath,
                clientIp,
                stopwatch.ElapsedMilliseconds
            );

            throw;
        }
    }
}
