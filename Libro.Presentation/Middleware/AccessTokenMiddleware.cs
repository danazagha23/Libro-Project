using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Libro.Presentation.Middleware
{
    public class AccessTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public AccessTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var accessToken = context.Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                context.Request.Headers.Add("Authorization", $"Bearer {accessToken}");
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
