namespace AIProductify.API.Middleware
{
    public class ApiKeyMiddleware
    {

        private readonly RequestDelegate _next;
        private const string ApiKeyHeaderName = "X-API-Key";



        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }



        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("API Key is missing.");
                return;
            }

            if (!IsValidApiKey(extractedApiKey))
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Invalid API Key.");
                return;
            }

            await _next(context);
        }

        private bool IsValidApiKey(string extractedApiKey)
        {
            return extractedApiKey == "OguzEvrensel";
        }
    }
}
