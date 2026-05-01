using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace UserManagementAPI.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log request
        context.Request.EnableBuffering();
        var requestBody = await ReadRequestBody(context.Request);
            _logger.LogInformation("Incoming Request {method} {path} Headers:{headers} Body:{body}",
            context.Request.Method,
            context.Request.Path + context.Request.QueryString,
            context.Request.Headers.ToDictionary(h => h.Key, h => string.Join(',', h.Value.ToArray())),
            requestBody);

        // Swap out response body to capture it
        var originalBodyStream = context.Response.Body;
        await using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);

            // Read response
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation("Outgoing Response {statusCode} Headers:{headers} Body:{body}",
                context.Response.StatusCode,
                context.Response.Headers.ToDictionary(h => h.Key, h => string.Join(',', h.Value.ToArray())),
                text);

            // Copy the contents of the new memory stream (which contains the response) to the original stream.
            await responseBody.CopyToAsync(originalBodyStream);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private static async Task<string> ReadRequestBody(HttpRequest request)
    {
        request.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Seek(0, SeekOrigin.Begin);
        return string.IsNullOrWhiteSpace(body) ? string.Empty : body;
    }
}
