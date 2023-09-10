namespace SuuchaStudio.Unity.Core.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEnableLogLevelContainer
    {
        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        /// <value>
        /// The log level.
        /// </value>
        LogLevel LogLevel { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Suucha.Unity.Core.Logging.IEnableLogLevelContainer" />
    public class EnableLogLevelContainer : IEnableLogLevelContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnableLogLevelContainer"/> class.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        public EnableLogLevelContainer(LogLevel logLevel)
        {
            LogLevel = logLevel;
        }
        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        /// <value>
        /// The log level.
        /// </value>
        public LogLevel LogLevel { get; set; }
    }
}
