using System;
using NLog;

namespace Logman.Common.Logging
{
    public class Nlogger : ILogger
    {
        private readonly Logger _internalLogger;

        public Nlogger()
        {
            _internalLogger = LogManager.GetCurrentClassLogger();
        }

        public void LogError(Exception exception)
        {
            _internalLogger.LogException(LogLevel.Error, exception.Message, exception);
        }

        public void LogDebugInfo(string message, LogSeverity severity = LogSeverity.Medium)
        {
            LogLevel logLevel;
            switch (severity)
            {
                case LogSeverity.High:
                    logLevel = LogLevel.Error;
                    break;
                case LogSeverity.Low:
                    logLevel = LogLevel.Info;
                    break;
                default:
                    logLevel = LogLevel.Warn;
                    break;
            }
            _internalLogger.Log(logLevel, message);
        }
    }
}