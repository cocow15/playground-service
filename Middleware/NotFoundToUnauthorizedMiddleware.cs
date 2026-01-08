using System.Net;

namespace AuthService.Middleware
{
    public class NotFoundToUnauthorizedMiddleware
    {
        private readonly RequestDelegate _next;

        public NotFoundToUnauthorizedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.NotFound && context.GetEndpoint() == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
        }
    }
}
