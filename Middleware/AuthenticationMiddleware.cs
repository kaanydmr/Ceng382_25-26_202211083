using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Week5.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _publicPaths;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
            _publicPaths = new[] { "/Login", "/Error", "/css", "/js", "/lib" };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip authentication for public paths
            if (IsPublicPath(context.Request.Path))
            {
                await _next(context);
                return;
            }

            // Check if the user is authenticated
            bool isAuthenticated = ValidateAuthentication(context);

            if (!isAuthenticated)
            {
                // Redirect to login page if not authenticated
                context.Response.Redirect("/Login");
                return;
            }

            await _next(context);
        }

        private bool IsPublicPath(PathString path)
        {
            foreach (var publicPath in _publicPaths)
            {
                if (path.StartsWithSegments(publicPath, out _))
                {
                    return true;
                }
            }
            return false;
        }

        private bool ValidateAuthentication(HttpContext context)
        {
            // Get values from session
            string sessionUsername = context.Session.GetString("username");
            string sessionToken = context.Session.GetString("token");
            string sessionId = context.Session.GetString("session_id");

            if (string.IsNullOrEmpty(sessionUsername) || 
                string.IsNullOrEmpty(sessionToken) || 
                string.IsNullOrEmpty(sessionId))
            {
                return false;
            }

            // Get values from cookies
            context.Request.Cookies.TryGetValue("username", out string cookieUsername);
            context.Request.Cookies.TryGetValue("token", out string cookieToken);
            context.Request.Cookies.TryGetValue("session_id", out string cookieSessionId);

            // Compare session and cookie values
            return sessionUsername == cookieUsername && 
                   sessionToken == cookieToken && 
                   sessionId == cookieSessionId;
        }
    }
}