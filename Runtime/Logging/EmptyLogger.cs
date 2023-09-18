namespace SuuchaStudio.Unity.Core.Logging
{
    /// <summary>
    /// Empty logger. Nothing to do.
    /// </summary>
    /// <seealso cref="Suucha.Unity.Core.Logging.LoggerAbstract" />
    public class EmptyLogger : LoggerAbstract
    {
        public EmptyLogger()
        {

        }
        private static ILogger instance;
        public static ILogger Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }
                instance = new EmptyLogger();
                return instance;
            }
        }

        protected override void LogInternal(LogLevel level, string tag, string format, params object[] args)
        {
            
        }
    }
}
