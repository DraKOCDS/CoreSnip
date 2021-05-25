using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreSnip.Serilog
{
    /// <summary>
    /// Request Logging Scope Enricher Middleware to add Request and User info to the logging scope
    /// </summary>
    public class RequestLoggingScopeEnricherMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingScopeEnricherMiddleware> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        public RequestLoggingScopeEnricherMiddleware(RequestDelegate next, ILogger<RequestLoggingScopeEnricherMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["RequestBody"] = await ReadRequestBody(context.Request),
                ["RequestHeaders"] = JsonSerializer.Serialize(context.Request.Headers),
                ["RequestMethod"] = context.Request.Method,
                ["User"] = context.User.Identity.Name
            }))
            {
                // Copy a pointer to the original response body stream
                var originalResponseBodyStream = context.Response.Body;

                // Create a new memory stream...
                using var responseBody = new MemoryStream();
                // ...and use that for the temporary response body
                context.Response.Body = responseBody;

                // Continue down the Middleware pipeline, eventually returning to this class
                await _next(context);

                // Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
                responseBody.Position = 0;
                await responseBody.CopyToAsync(originalResponseBodyStream);
            }
        }

        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();

            using var streamReader = new StreamReader(request.Body, leaveOpen: true);
            string requestBody = await streamReader.ReadToEndAsync();

            // Reset the request's body stream position for next middleware in the pipeline.
            request.Body.Position = 0;

            return requestBody;
        }
    }
}
