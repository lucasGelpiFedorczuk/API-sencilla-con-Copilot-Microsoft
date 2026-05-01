using Microsoft.AspNetCore.Builder;

namespace UserManagementAPI.Middleware;

public static class TokenAuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseTokenAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenAuthenticationMiddleware>();
    }
}
