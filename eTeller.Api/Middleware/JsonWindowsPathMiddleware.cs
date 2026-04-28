using System.Text;
using System.Text.RegularExpressions;

namespace eTeller.Api.Middleware
{
    public partial class JsonWindowsPathMiddleware
    {
        private readonly RequestDelegate _next;

        public JsonWindowsPathMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (IsJsonBody(context.Request))
            {
                context.Request.EnableBuffering();

                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Seek(0, SeekOrigin.Begin);

                var fixedBody = InvalidEscapeRegex().Replace(body, _ => @"\\");

                if (fixedBody.Length != body.Length)
                {
                    var bytes = Encoding.UTF8.GetBytes(fixedBody);
                    context.Request.Body = new MemoryStream(bytes);
                    context.Request.ContentLength = bytes.Length;
                }
            }

            await _next(context);
        }

        private static bool IsJsonBody(HttpRequest request) =>
            request.Method is "POST" or "PUT" or "DELETE" &&
            request.ContentType?.Contains("application/json", StringComparison.OrdinalIgnoreCase) == true;

        // Matches \ NOT followed by a valid JSON escape character: " \ / b f n r t u
        [GeneratedRegex(@"\\(?![""\\\/bfnrtu])")]
        private static partial Regex InvalidEscapeRegex();
    }
}
