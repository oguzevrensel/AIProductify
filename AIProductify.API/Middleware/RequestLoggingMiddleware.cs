using Serilog;
using System.Text;


public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path;
        var query = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null;

        string requestBody = await ReadRequestBody(context.Request);

        var originalResponseBodyStream = context.Response.Body;
        using var newResponseBodyStream = new MemoryStream();
        context.Response.Body = newResponseBodyStream;

        try
        {
            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await ReadStreamAsync(context.Response.Body);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await newResponseBodyStream.CopyToAsync(originalResponseBodyStream);

            Log.Information(
                "HTTP Request: Path={Path}, Query={Query}, RequestBody={RequestBody}, StatusCode={StatusCode}, ResponseBody={ResponseBody}",
                path, query, requestBody, context.Response.StatusCode, responseBody
            );
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An exception occurred while processing the request.");
            throw;
        }
    }

    private async Task<string> ReadStreamAsync(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        var result = await reader.ReadToEndAsync();
        stream.Seek(0, SeekOrigin.Begin); // Reset stream position for reusability
        return result;
    }

    private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
            var requestBody = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);
            return requestBody;
        }
}
