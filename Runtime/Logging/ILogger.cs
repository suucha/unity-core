namespace SuuchaStudio.Unity.Core.Logging
{
    /// <summary>
    /// Interface of logger
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Gets or sets the enable log level.
        /// </summary>
        /// <value>
        /// The enable log level.
        /// </value>
        LogLevel EnableLogLevel { get; }
        /// <summary>
        /// Logs the specified log level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        void Log(LogLevel logLevel, string format, params object[] args);
        /// <summary>
        /// Logs the trace.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        void LogTrace(string format, params object[] args);
        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        void LogDebug(string format, params object[] args);
        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        void LogInformation(string format, params object[] args);
        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        void LogWarning(string format, params object[] args);
        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        void LogError(string format, params object[] args);
        /// <summary>
        /// Logs the critical.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        void LogCritical(string format, params object[] args);
    }
}
