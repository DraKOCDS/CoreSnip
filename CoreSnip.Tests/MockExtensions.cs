using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace CoreSnip.Tests
{
    public static class MockExtensions
    {
        /// <summary>
        /// Verify that the <paramref name="logger"/> calls the Log method with the <paramref name="expectedMessage"/> and <paramref name="expectedLogLevel"/> the specified number of <paramref name="times"/>
        /// </summary>
        /// <typeparam name="T">Logger type</typeparam>
        /// <param name="logger">Mocked logger</param>
        /// <param name="expectedMessage">The expected message logged</param>
        /// <param name="expectedLogLevel">The expected logging level</param>
        /// <param name="times">The expected number of times the specified log need to be written</param>
        /// <returns>The same <paramref name="logger"/></returns>
        public static Mock<ILogger<T>> VerifyLogging<T>(this Mock<ILogger<T>> logger, string expectedMessage, LogLevel expectedLogLevel = LogLevel.Debug, Times? times = null)
        {
            times ??= Times.Once();

            Func<object, Type, bool> state = (v, t) => v.ToString().CompareTo(expectedMessage) == 0;

            logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == expectedLogLevel),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => state(v, t)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)), (Times)times);

            return logger;
        }

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
