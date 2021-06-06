using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;

namespace CoreSnip.Tests.MockExtensions
{
    public static class MockServiceProviderExtensions
    {
        /// <summary>
        /// Set up the internal <see cref="IServiceScope"/> and <see cref="IServiceScopeFactory"/> with mocked objects 
        /// so that subsequent set up of the GetService method on the <paramref name="serviceProvider"/> will return the desired mock. 
        /// <para><c>
        /// serviceProvider.Setup(x => x.GetService(typeof(IMyInterface))).Returns(myInterface.Object);
        /// </c></para>
        /// </summary>
        /// <param name="serviceProvider">The service provider to set up</param>
        /// <returns>The configured service provider mock</returns>
        public static Mock<IServiceProvider> SetupBaseInfrastructure(this Mock<IServiceProvider> serviceProvider)
        {
            var serviceScope = new Mock<IServiceScope>();
            var serviceScopeFactory = new Mock<IServiceScopeFactory>();

            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);
            serviceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);
            serviceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);

            return serviceProvider;
        }
    }
}
