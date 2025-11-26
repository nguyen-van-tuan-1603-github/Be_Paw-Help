namespace PawHelp.Middleware;

public class AdminAuthMiddleware
{
    private readonly RequestDelegate _next;

    public AdminAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower() ?? "";

        // Kiểm tra nếu đang truy cập trang Admin
        if (path.StartsWith("/admin"))
        {
            // Kiểm tra session đăng nhập
            var isAuthenticated = context.Session.GetString("IsAuthenticated");
            
            if (string.IsNullOrEmpty(isAuthenticated) || isAuthenticated != "true")
            {
                // Chưa đăng nhập -> redirect về trang login
                context.Response.Redirect($"/Auth/Login?returnUrl={Uri.EscapeDataString(path)}");
                return;
            }
        }

        await _next(context);
    }
}

// Extension method để dễ dàng sử dụng middleware
public static class AdminAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseAdminAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AdminAuthMiddleware>();
    }
}

