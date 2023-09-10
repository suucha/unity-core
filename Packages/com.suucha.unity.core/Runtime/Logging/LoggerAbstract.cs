
using SuuchaStudio.Unity.Core.Ioc;

namespace SuuchaStudio.Unity.Core.Logging
{
    public abstract class LoggerAbstract : ILogger
    {
        private IEnableLogLevelContainer enableLogLevelContainer;

        public string Tag { get; set; }
        /// <summary>
        /// Gets or sets the enable log level.
        /// </summary>
        /// <value>
        /// The enable log level.
        /// </value>
        public LogLevel EnableLogLevel
        {
            get
            {
                if (enableLogLevelContainer != null)
                {
                    return enableLogLevelContainer.LogLevel;
                }
                if (IocContainer.TryResolve<IEnableLogLevelContainer>(out enableLogLevelContainer))
                {
                    return enableLogLevelContainer.LogLevel;
                }
                return LogLevel.Trace;
            }
        }
        /// <summary>
        /// Logs the specified log level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public void Log(LogLevel logLevel, string format, params object[] args)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    LogDebug(format, args);
                    break;
                case LogLevel.Information:
                    LogInformation(format, args);
                    break;
                case LogLevel.Warning:
                    LogWarning(format, args);
                    break;
                case LogLevel.Error:
                    LogError(format, args);
                    break;
                case LogLevel.Critical:
                    LogCritical(format, args);
                    break;
                default:
                    LogTrace(format, args);
                    break;
            }
        }
        /// <summary>
        /// Logs the critical.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public void LogCritical(string format, params object[] args)
        {
            if (EnableLogLevel <= LogLevel.Critical)
            {
                LogInternal(LogLevel.Critical, Tag, "CRITICAL " + format, args);
            }
        }
        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public void LogDebug(string format, params object[] args)
        {
            if (EnableLogLevel <= LogLevel.Debug)
            {
                LogInternal( LogLevel.Debug, Tag, "DEBUG " + format, args);
            }
        }
        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public void LogError(string format, params object[] args)
        {
            if (EnableLogLevel <= LogLevel.Error)
            {
                LogInternal(LogLevel.Error, Tag, "ERROR " + format, args);
            }
        }
        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public void LogInformation(string format, params object[] args)
        {
            if (EnableLogLevel <= LogLevel.Information)
            {
                LogInternal( LogLevel.Information, Tag, "INFO " + format, args);
            }
        }
        /// <summary>
        /// Logs the trace.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public void LogTrace(string format, params object[] args)
        {
            if (EnableLogLevel <= LogLevel.Trace)
            {
                LogInternal(LogLevel.Trace, Tag, "TRACE " + format, args);
            }
        }
        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public void LogWarning(string format, params object[] args)
        {
            if (EnableLogLevel <= LogLevel.Warning)
            {
                LogInternal(LogLevel.Warning, Tag, "WARN " + format, args);
            }
        }
        protected abstract void LogInternal(LogLevel level, string tag, string format, params object[] args);
    }
}
