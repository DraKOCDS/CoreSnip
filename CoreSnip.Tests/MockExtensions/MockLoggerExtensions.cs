using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace CoreSnip.Tests.MockExtensions
{
    public static class MockLoggerExtensions
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
    }
}
