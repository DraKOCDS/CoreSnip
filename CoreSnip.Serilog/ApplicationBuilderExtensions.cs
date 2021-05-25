using Microsoft.AspNetCore.Builder;
using Serilog;

namespace CoreSnip.Serilog
{
    /// <summary>
    /// <see cref="IApplicationBuilder"/> extension methods to add custom middlewares
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds the custom <see cref="RequestLoggingScopeEnricherMiddleware"/> middleware and the the Serilog Request Logging 
        /// enriching the diagnostic context with the HTTP Request and Response body and headers. <see cref="SerilogLoggerHelper.EnrichFromResponse(IDiagnosticContext, Microsoft.AspNetCore.Http.HttpContext)"/>
        /// <para>The middleware adds the fallowing value to the logging scope/Serilog diagnostic context:</para>
        /// RequestBody, RequestHeaders, RequestMethod, User, ResponseBody, ResponseHeaders
        /// </summary>
        /// <param name="app"></param>
        public static void UseSerilogRequestResponseLogging(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestLoggingScopeEnricherMiddleware>();
            app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = SerilogEnrichHelper.EnrichFromResponse);
        }
    }
}
