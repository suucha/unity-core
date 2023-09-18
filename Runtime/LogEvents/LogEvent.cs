using System;
using System.Collections.Generic;

namespace SuuchaStudio.Unity.Core.LogEvents
{
    internal class LogEvent
    {
        public string Name { get; }
        public Dictionary<string, string> Parameters { get; }
        public List<LogEventNameWithInterceptName> NewNames { get; }

        public LogEvent(string name, Dictionary<string, string> parameters)
        {
            Name = name;
            Parameters = parameters;
            NewNames = new List<LogEventNameWithInterceptName>();
        }
        public void AddNewEventName(string newEventName, string interceptorName)
        {
            NewNames.Add(new LogEventNameWithInterceptName(newEventName, interceptorName));
        }
    }
    internal class LogEventNameWithInterceptName
    {
        public string Name { get; }
        public string InterceptorName { get; }
        public LogEventNameWithInterceptName(string name, string interceptorName)
        {
            Name = name;
            InterceptorName = interceptorName;
        }

    }
}
