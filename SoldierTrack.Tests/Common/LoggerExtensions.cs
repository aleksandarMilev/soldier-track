namespace SoldierTrack.Tests.Common
{
    using Microsoft.Extensions.Logging;
    using Moq;

    public static class LoggerExtensions
    {
        public static void VerifyLogged<T>(
            this Mock<ILogger<T>> logger,
            LogLevel level)
        {
            logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == level),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v != null),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeastOnce());
        }
    }
}