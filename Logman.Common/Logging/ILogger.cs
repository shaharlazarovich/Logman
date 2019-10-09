using System;

namespace Logman.Common.Logging
{
    public interface ILogger
    {
        void LogError(Exception exception);
        void LogDebugInfo(string message, LogSeverity severity = LogSeverity.Medium);
    }
}