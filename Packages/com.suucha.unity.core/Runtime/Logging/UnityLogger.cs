using UnityEngine;

namespace SuuchaStudio.Unity.Core.Logging
{
    public class UnityLogger : LoggerAbstract
    {
        protected override void LogInternal(LogLevel level, string tag, string format, params object[] args)
        {
            var formatWithTag = format;
            if (!string.IsNullOrEmpty(tag))
            {
                formatWithTag = "[" + tag + "] " + format;
            }
            switch(level)
            {
                case LogLevel.Debug:
                case LogLevel.Information:
                case LogLevel.Trace:
                    if (args == null || args.Length == 0)
                    {
                        Debug.Log(formatWithTag);
                        return;
                    }
                    Debug.LogFormat(formatWithTag, args);
                    break;
                case LogLevel.Warning:
                    if (args == null || args.Length == 0)
                    {
                        Debug.LogWarning(formatWithTag);
                        return;
                    }
                    Debug.LogWarningFormat(formatWithTag, args);
                    break;
                case LogLevel.Error:
                case LogLevel.Critical:
                    if (args == null || args.Length == 0)
                    {
                        Debug.LogError(formatWithTag);
                        return;
                    }
                    Debug.LogErrorFormat(formatWithTag, args);
                    break;
            }
        }
    }
}
