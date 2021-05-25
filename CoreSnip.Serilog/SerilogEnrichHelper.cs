using Microsoft.AspNetCore.Http;
using Serilog;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreSnip.Serilog
{
    /// <summary>
    /// Serilog helper to enrich the <see cref="IDiagnosticContext"/> with response body and headers
    /// </summary>
    public class SerilogEnrichHelper
    {
        /// <summary>
        /// Adds <see cref="HttpContext.Response"/> Body and Headers to the <see cref="IDiagnosticContext"/>
        /// </summary>
        /// <param name="diagnosticContext"></param>
        /// <param name="httpContext"></param>
        public static async void EnrichFromResponse(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            string responseBody = await ReadResponseBody(httpContext.Response);
            diagnosticContext.Set("ResponseBody", responseBody);
            diagnosticContext.Set("ResponseHeaders", JsonSerializer.Serialize(httpContext.Response.Headers));
        }

        private static async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            string responseBody = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return responseBody;
        }
    }
}
