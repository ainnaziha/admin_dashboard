namespace spl.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            bool isAuthenticated = context.Request.Cookies["IsAuthenticated"] == "1";

            if (isAuthenticated && (context.Request.Path.Equals("/index", StringComparison.OrdinalIgnoreCase)
                || context.Request.Path.Equals("/", StringComparison.OrdinalIgnoreCase)))
            {
                context.Response.Redirect("/dashboard");
            }
            else if (!isAuthenticated && !context.Request.Path.Equals("/index", StringComparison.OrdinalIgnoreCase) 
                && !context.Request.Path.Equals("/error", StringComparison.OrdinalIgnoreCase) && !context.Request.Path.Equals("/", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.Redirect("/");
            }
            else
            {
                await _next(context);
            }
        }
    }

}
