using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace UserManagementAPI.Middleware;

public class TokenAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenAuthenticationMiddleware> _logger;
    private readonly string _expectedToken;

    public TokenAuthenticationMiddleware(RequestDelegate next, ILogger<TokenAuthenticationMiddleware> logger, IConfiguration config)
    {
        _next = next;
        _logger = logger;
        _expectedToken = config["Auth:Token"] ?? string.Empty;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = "Missing Authorization header" });
            return;
        }

        var header = authHeader.ToString();
        if (!header.StartsWith("Bearer "))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = "Invalid Authorization scheme" });
            return;
        }

        var token = header.Substring("Bearer ".Length).Trim();
        if (string.IsNullOrEmpty(_expectedToken) || token != _expectedToken)
        {
            _logger.LogWarning("Unauthorized request with token: {token}", token);
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = "Invalid token" });
            return;
        }

        await _next(context);
    }
}
