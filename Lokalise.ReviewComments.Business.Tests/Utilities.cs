using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;

namespace Lokalise.ReviewComments.Business.Tests;

public static class Utilities
{
    internal static T DeepClone<T>(T source)
    {
        var text = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(text);
    }

    internal static void VerifyLog<T>(Mock<ILogger<T>> logger, LogLevel logLevel, string message, int times = 1)
    {
        logger.Verify(l => l.Log(
            It.Is<LogLevel>(l => l == logLevel),
            It.Is<EventId>(e => e.Id == 0),
            It.Is<It.IsAnyType>((o, @t) => string.Equals(o.ToString(), message, StringComparison.Ordinal) && @t.Name == "FormattedLogValues"),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                times == 0 ? Times.Never() : times == 1 ? Times.Once() : Times.Exactly(times));
    }
}