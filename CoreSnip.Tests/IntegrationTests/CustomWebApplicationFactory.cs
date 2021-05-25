using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CoreSnip.Tests.IntegrationTests
{
    /// <summary>
    /// A <see cref="WebApplicationFactory{TStartup}"/> to build a server for integration testing with fake services 
    /// </summary>
    /// <typeparam name="TStartup">The Startup class</typeparam>
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public Action<IServiceCollection> Registrations { get; set; }
        public IAuthenticatedUserInfo AuthenticatedUserInfo { get; set; }

        public CustomWebApplicationFactory() : this(null, null)
        {
        }

        /// <summary>
        /// Create fake server instance with optional fake authenticated and authorized user and test services
        /// </summary>
        /// <param name="authenticatedUserInfo">User information to fake authentication and authorization</param>
        /// <param name="registrations">Service collaction registration where you can swap the real server registered in the <see cref="TStartup"/> class with fake services</param>
        public CustomWebApplicationFactory(IAuthenticatedUserInfo authenticatedUserInfo = null, Action<IServiceCollection> registrations = null)
        {
            AuthenticatedUserInfo = authenticatedUserInfo;
            Registrations = registrations ?? (collection => { });
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                if (AuthenticatedUserInfo != null)
                {
                    services.AddSingleton(AuthenticatedUserInfo);
                    services.AddSingleton<IPolicyEvaluator, FakeAuthPolicyEvaluator>();
                }

                Registrations?.Invoke(services);
            });
        }
    }
}
