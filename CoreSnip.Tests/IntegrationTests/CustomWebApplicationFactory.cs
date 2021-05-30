using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace CoreSnip.Tests.IntegrationTests
{
    /// <summary>
    /// A <see cref="WebApplicationFactory{TStartup}"/> to build a custom test server
    /// </summary>
    /// <typeparam name="TStartup">The Startup class</typeparam>
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public Action<IServiceCollection> ConfigureTestServices { get; set; }
        public IAuthenticatedUserInfo AuthenticatedUserInfo { get; set; }

        /// <summary>
        /// Create integration test server instance
        /// </summary>
        /// <param name="environmentName">Used to set the ASPNETCORE_ENVIRONMENT value</param>
        public CustomWebApplicationFactory(string environmentName = "Tests") : this(null, null, environmentName) {}

        /// <summary>
        /// Create integration test server instance with fake authenticated and authorized user
        /// </summary>
        /// <param name="authenticatedUserInfo">User information to fake authentication and authorization</param>
        /// <param name="environmentName">Used to set the ASPNETCORE_ENVIRONMENT value</param>
        public CustomWebApplicationFactory(IAuthenticatedUserInfo authenticatedUserInfo, string environmentName = "Tests") : this(authenticatedUserInfo, null, environmentName) {}

        /// <summary>
        /// Create integration test server instance with test services
        /// </summary>
        /// <param name="configureTestServices">Service collaction registration where you can swap the real server registered in the <see cref="TStartup"/> class with fake services</param>
        /// <param name="environmentName">Used to set the ASPNETCORE_ENVIRONMENT value</param>
        public CustomWebApplicationFactory(Action<IServiceCollection> configureTestServices, string environmentName = "Tests") : this(null, configureTestServices, environmentName) { }

        /// <summary>
        /// Create integration test server instance with fake authenticated and authorized user and test services
        /// </summary>
        /// <param name="authenticatedUserInfo">User information to fake authentication and authorization</param>
        /// <param name="configureTestServices">Service collaction registration where you can swap the real server registered in the <see cref="TStartup"/> class with fake services</param>
        /// <param name="environmentName">Used to set the ASPNETCORE_ENVIRONMENT value</param>
        public CustomWebApplicationFactory(IAuthenticatedUserInfo authenticatedUserInfo, Action<IServiceCollection> configureTestServices, string environmentName = "Tests")
        {
            //this is needed if appsettings.{_hostingEnvironment.EnvironmentName}.json is used in the Startup class
            //to avoid loading different environment settings
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environmentName);

            AuthenticatedUserInfo = authenticatedUserInfo;
            ConfigureTestServices = configureTestServices ?? (collection => { });
        }

        //needed to workaround deadloack issue on weak machine
        //refer to: https://www.strathweb.com/2021/05/the-curious-case-of-asp-net-core-integration-test-deadlock/
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = builder.Build();
            Task.Run(() => host.StartAsync()).GetAwaiter().GetResult();
            return host;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //set the content root to the test project folder so that appsettings.json in the test project will be picked
            builder.UseContentRoot(".");

            builder.ConfigureTestServices(services =>
            {
                if (AuthenticatedUserInfo != null)
                {
                    services.AddSingleton(AuthenticatedUserInfo);
                    services.AddSingleton<IPolicyEvaluator, FakeAuthPolicyEvaluator>();
                }

                ConfigureTestServices?.Invoke(services);
            });
        }
    }
}
