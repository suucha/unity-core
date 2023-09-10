using Cysharp.Threading.Tasks;
using SuuchaStudio.Unity.Core.LogEvents.Intercepts;
using System;
using System.Collections.Generic;

namespace SuuchaStudio.Unity.Core.LogEvents
{
    public static class SuuchaLogEventExtension
    {
        /// <summary>
        /// Adds a log event reporter to the Suucha instance.
        /// </summary>
        /// <typeparam name="TLogEventReporter">The type of the log event reporter. This type must implement <seealso cref="ILogEventReporter"/>.</typeparam>
        /// <param name="suucha">The Suucha instance to which the log event reporter is added.</param>
        /// <param name="logEventReporter">The log event reporter to add.</param>
        /// <returns>The added log event reporter.</returns>
        public static TLogEventReporter AddLogEventReporter<TLogEventReporter>(this Suucha suucha, TLogEventReporter logEventReporter)
            where TLogEventReporter : ILogEventReporter
        {
            LogEventManager.AddReporter(logEventReporter).GetAwaiter().GetResult();
            return logEventReporter;
        }

        public static Suucha AddLogEventIntercept(this Suucha suucha, ILogEventIntercept logEventIntercept)
        {
            LogEventManager.AddEventInterceptor(logEventIntercept);
            return suucha;
        }

        public static Suucha AddLogEventParameterIntercept(this Suucha suucha, ILogEventParameterIntercept logEventParameterIntercept)
        {
            LogEventManager.AddEventParameterInterceptor(logEventParameterIntercept);
            return suucha;
        }
        public static Suucha EnableRunDurationEventParameter(this Suucha suucha)
        {
            LogEventManager.AddEventParameterInterceptor(new AppRunDurationEventParameterIntercept());
            return suucha;
        }
        public static Suucha AddEventCounter(this Suucha suucha, List<EventCounterConfiguration> eventCounterConfigurations)
        {
            var intercept = new EventCounterIntercept(eventCounterConfigurations);
            LogEventManager.AddEventInterceptor(intercept);
            return suucha;
        }
        /// <summary>
        /// Logs an event on the Suucha instance.
        /// </summary>
        /// <param name="suucha">The Suucha instance on which the event is logged.</param>
        /// <param name="name">The name of the event.</param>
        /// <param name="eventParameters">The parameters of the event. If this is null, an empty dictionary will be used.</param>
        /// <returns>A UniTask that represents the asynchronous logging operation.</returns>
        public static UniTask LogEvent(this Suucha suucha, string name, Dictionary<string, string> eventParameters = null)
        {
            eventParameters ??= new Dictionary<string, string>();
            return LogEventManager.LogEvent(name, eventParameters);
        }

        public static TLogEventReporter EnableAbGroupEvent<TLogEventReporter>(this TLogEventReporter logEventReporter)
            where TLogEventReporter : ILogEventReporter
        {
            logEventReporter.EnableEventIntercept<AbGroupEventIntercept>();
            return logEventReporter;
        }
        public static TLogEventReporter EnableEventCounter<TLogEventReporter>(this TLogEventReporter logEventReporter)
            where TLogEventReporter : ILogEventReporter
        {
            logEventReporter.EnableEventIntercept<EventCounterIntercept>();
            return logEventReporter;
        }
        public static void Use<TLogEventReporter>(this TLogEventReporter logEventReporter) 
            where TLogEventReporter : ILogEventReporter
        {
            Suucha.App.AddLogEventReporter(logEventReporter);
        }
        public static TLogEventReporter AddRequiredEventParameters<TLogEventReporter>(this TLogEventReporter logEventReporter,
            Dictionary<string, string> requiredParameters,
            int timeout) where TLogEventReporter : ILogEventReporter
        {
            logEventReporter.AddRequiredParameters(requiredParameters, timeout);
            return logEventReporter;
        }
        //public static TLogEventReporter EnableEventCounter<TLogEventReporter>(this TLogEventReporter logEventReporter, List<EventCounterConfiguration> eventCounterConfigurations)
        //    where TLogEventReporter : ILogEventReporter
        //{
        //    var intercept = new EventCounterIntercept(eventCounterConfigurations);
        //    //logEventReporter.AddEventIntercept(intercept);
        //    return logEventReporter;
        //}
    }
}
