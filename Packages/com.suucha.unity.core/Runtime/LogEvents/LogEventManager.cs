using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace SuuchaStudio.Unity.Core.LogEvents
{
    internal class LogEventManager
    {
        private const int DefaultCacheSize = 100;
        private static readonly Queue<LogEvent> eventCache = new Queue<LogEvent>(DefaultCacheSize);
        private static readonly List<ILogEventReporter> reporters = new List<ILogEventReporter>();
        private static readonly List<ILogEventIntercept> eventInterceptors = new List<ILogEventIntercept>();
        private static readonly List<ILogEventParameterIntercept> eventParameterInterceptors = new List<ILogEventParameterIntercept>();

        /// <summary>
        /// Adds a log event reporter.
        /// </summary>
        /// <param name="reporter">The log event reporter to add.</param>
        public static async UniTask AddReporter(ILogEventReporter reporter)
        {
            // Check if a reporter with the same name already exists.
            if (reporters.Any(r => r.Name == reporter.Name
              && reporter.GetType().FullName == r.GetType().FullName))
            {
                //throw new ArgumentException($"A reporter with the name {reporter.Name} has already been added.");
                return;
            }

            reporters.Add(reporter);

            // Report cached events to the new reporter.
            foreach (var logEvent in eventCache)
            {
                await ReportLogEvent(reporter, logEvent);
            }
        }

        private static async UniTask ReportLogEvent(ILogEventReporter reporter, LogEvent logEvent)
        {
            if (!reporter.IsAllowEventReport(logEvent.Name))
            {
                return;
            }
            
            await reporter.LogEvent(logEvent.Name, logEvent.Parameters);
            var reportedEventNames = new List<string> { logEvent.Name };
            foreach (var newLogEventName in logEvent.NewNames)
            {
                if (reporter.IsEnableEventIntercept(newLogEventName.InterceptorName)
                    && !reportedEventNames.Contains(newLogEventName.Name))
                {
                    await reporter.LogEvent(newLogEventName.Name, logEvent.Parameters);
                    reportedEventNames.Add(newLogEventName.Name);
                }
            }
        }

        /// <summary>
        /// Logs an event.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="parameters">The parameters of the event.</param>
        public static async UniTask LogEvent(string name, Dictionary<string, string> parameters = null)
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }
            var clonedEventParameters = new Dictionary<string, string>(parameters);
            var orderedParameterIntercepts = eventParameterInterceptors.OrderByDescending(i => i.Order);
            foreach (var parameterIntercept in eventParameterInterceptors)
            {
                var newParameters = await parameterIntercept.Execute(name, parameters);
                foreach (var parameter in newParameters)
                {
                    if (clonedEventParameters.ContainsKey(parameter.Key))
                    {
                        clonedEventParameters[parameter.Key] = parameter.Value;
                    }
                    else
                    {
                        clonedEventParameters.Add(parameter.Key, parameter.Value);
                    }
                }
            }
            var logEvent = new LogEvent(name, clonedEventParameters);

            // Apply all interceptors to the event.
            foreach (var interceptor in eventInterceptors)
            {
                var newLogEventNames = await interceptor.Execute(name, clonedEventParameters);
                var interceptorType = interceptor.GetType();
                var interceptorTypeName = $"{interceptorType.Namespace}.{interceptorType.Name}";
                foreach (var logEventName in newLogEventNames)
                {
                    if (logEventName != name)
                    {
                        logEvent.AddNewEventName(logEventName, interceptorTypeName);
                    }
                }
            }

            if (eventCache.Count == DefaultCacheSize)
            {
                eventCache.Dequeue();
            }
            eventCache.Enqueue(logEvent);


            // Report the event to all reporters.
            foreach (var reporter in reporters)
            {
                await ReportLogEvent(reporter, logEvent);
            }
        }

        public static void AddEventInterceptor(ILogEventIntercept interceptor)
        {
            if (interceptor == null 
                || eventInterceptors.FirstOrDefault(e=>e.Name == interceptor.Name) != null)
            {
                return;
            }
            eventInterceptors.Add(interceptor);

            // Sort interceptors by order.
            eventInterceptors.Sort((a, b) => a.Order.CompareTo(b.Order));
        }
        public static void AddEventParameterInterceptor(ILogEventParameterIntercept interceptor)
        {
            if (interceptor == null
                || eventParameterInterceptors.FirstOrDefault(e => e.Name == interceptor.Name) != null)
            {
                return;
            }
            eventParameterInterceptors.Add(interceptor);
            // Sort interceptors by order.
            eventParameterInterceptors.Sort((a, b) => a.Order.CompareTo(b.Order));
        }
    }



    /*
    /// <summary>
    /// 有些事件拦截器数据是全局的，比如事件计数，如果开启了此拦截器的事件上报器
    /// </summary>
    internal class LogEventManager : SuuchaBase
    {
        private static LogEventManager logEventReporter;
        private static readonly List<PendingEvent> pendingEvents = new List<PendingEvent>();
        private readonly List<ILogEventReporter> eventReporters;
        private readonly List<ILogEventIntercept> intercepts;
        private readonly List<ILogEventParameterIntercept> parameterIntercepts;
        private readonly List<string> abGroupEventNames;
        private CommonEventParameter commonEventParameter;
        private LogEventManager()
        {
            eventReporters = new List<ILogEventReporter>();
            intercepts = new List<ILogEventIntercept>();
            parameterIntercepts = new List<ILogEventParameterIntercept>();
            abGroupEventNames = new List<string>();
            commonEventParameter = new CommonEventParameter();
        }
        public static LogEventManager Initialize()
        {
            return Initialize(null);
        }
        public static LogEventManager Initialize(CommonEventParameter commonEventParameter)
        {
            if (logEventReporter == null)
            {
                logEventReporter = new LogEventManager();
            }
            logEventReporter.commonEventParameter = commonEventParameter;
            if (logEventReporter.commonEventParameter == null)
            {
                logEventReporter.commonEventParameter = new CommonEventParameter();
            }
            return logEventReporter;
        }
        public static void ChangeEnabledEventNames(string reporterName, params string[] eventNames)
        {
            if (logEventReporter == null)
            {
                logEventReporter = new LogEventManager();
            }
            var reporter = logEventReporter.eventReporters.FirstOrDefault(er => er.Name == reporterName);
            if (reporter == null)
            {
                return;
            }
            reporter.ChangeEnabledEventNames(eventNames);
        }
        public static void AddCommonEventParameter(string parameterName, string value)
        {
            if (logEventReporter == null)
            {
                logEventReporter = new LogEventManager();
            }
            if (logEventReporter.commonEventParameter.ContainsKey(parameterName))
            {
                logEventReporter.commonEventParameter.Remove(parameterName);
            }
            logEventReporter.commonEventParameter.Add(parameterName, value);
            foreach (var eventReporter in logEventReporter.eventReporters)
            {
                eventReporter.AddCommonEventParameter(parameterName, value);
            }
            foreach (var pendingEvent in pendingEvents)
            {
                if (!pendingEvent.EventParameters.ContainsKey(parameterName))
                {
                    pendingEvent.EventParameters[parameterName] = value;
                }
            }
        }
        public static void EnableAbGroup(List<string> eventNames)
        {
            if (logEventReporter == null)
            {
                logEventReporter = new LogEventManager();
            }
            foreach (var eventName in eventNames)
            {
                if (!logEventReporter.abGroupEventNames.Contains(eventName))
                {
                    logEventReporter.abGroupEventNames.Add(eventName);
                }
            }
        }
        public static void AddParameterIntercept(ILogEventParameterIntercept parameterIntercept)
        {
            if (logEventReporter == null)
            {
                logEventReporter = new LogEventManager();
            }
            if (parameterIntercept == null)
            {
                return;
            }
            var oldIntercept = logEventReporter.parameterIntercepts.FirstOrDefault(i => i.Name == parameterIntercept.Name);
            if (oldIntercept != null)
            {
                logEventReporter.parameterIntercepts.Remove(oldIntercept);
            }
            logEventReporter.parameterIntercepts.Add(parameterIntercept);
        }
        public static void AddIntercept(ILogEventIntercept intercept)
        {
            if (logEventReporter == null)
            {
                logEventReporter = new LogEventManager();
            }
            if (intercept == null)
            {
                return;
            }
            var oldIntercept = logEventReporter.intercepts.FirstOrDefault(i => i.Name == intercept.Name);
            if (oldIntercept != null)
            {
                logEventReporter.intercepts.Remove(oldIntercept);
            }
            logEventReporter.intercepts.Add(intercept);
        }
        public static void AddEventReporter(ILogEventReporter eventReporter,
            List<string> eventNames = null,
            Dictionary<string, string> eventNameMap = null,
            Dictionary<string, string> eventParameterNameMap = null)
        {
            if (eventReporter == null)
            {
                return;
            }
            if (logEventReporter == null)
            {
                logEventReporter = new LogEventManager();
            }
            logEventReporter.Logger.LogInformation($"Add event reporter, Name:{eventReporter.Name}.");


            var old = logEventReporter.eventReporters.FirstOrDefault(e => e.Name == eventReporter.Name);
            if (old != null)
            {
                return;
            }
            if (eventNameMap == null)
            {
                eventNameMap = new Dictionary<string, string>();
            }
            if (eventParameterNameMap == null)
            {
                eventParameterNameMap = new Dictionary<string, string>();
            }
            eventReporter.Initialize(eventNames,
                eventNameMap,
                eventParameterNameMap,
                new List<string>());
            foreach (var eventParameter in logEventReporter.commonEventParameter)
            {
                eventReporter.AddCommonEventParameter(eventParameter.Key, eventParameter.Value);
            }
            logEventReporter.eventReporters.Add(eventReporter);
            foreach (var pendingEvent in pendingEvents)
            {
                logEventReporter.Logger.LogInformation($"Send pending event:{pendingEvent.EventName} to {eventReporter.Name}");
                eventReporter.LogEvent(pendingEvent.EventName, pendingEvent.EventParameters);
            }

        }
        public static void ChangeEventNameMap(string reporterName, Dictionary<string, string> eventNameMap)
        {
            if (logEventReporter == null)
            {
                logEventReporter = new LogEventManager();
            }
            var reporter = logEventReporter.eventReporters.FirstOrDefault(er => er.Name == reporterName);
            if (reporter == null)
            {
                return;
            }
            reporter.ChangeEventNameMap(eventNameMap);
        }

        public static void ChangeEventParameterNameMap(string reporterName, Dictionary<string, string> eventParameterNameMap)
        {
            if (logEventReporter == null)
            {
                logEventReporter = new LogEventManager();
            }
            var reporter = logEventReporter.eventReporters.FirstOrDefault(er => er.Name == reporterName);
            if (reporter == null)
            {
                return;
            }
            reporter.ChangeEventParameterNameMap(eventParameterNameMap);
        }
        public static async UniTask LogEvent(string name, Dictionary<string, string> eventParameters)
        {
            if (eventParameters == null)
            {
                eventParameters = new Dictionary<string, string>();
            }
            var clonedEventParameters = new Dictionary<string, string>(eventParameters);
            var orderedParameterIntercepts = logEventReporter.parameterIntercepts.OrderByDescending(i => i.Order);
            foreach (var parameterIntercept in orderedParameterIntercepts)
            {
                var parameters = await parameterIntercept.Execute(name, eventParameters);
                foreach (var parameter in parameters)
                {
                    if (clonedEventParameters.ContainsKey(parameter.Key))
                    {
                        clonedEventParameters[parameter.Key] = parameter.Value;
                    }
                    else
                    {
                        clonedEventParameters.Add(parameter.Key, parameter.Value);
                    }
                }
            }
            if (pendingEvents.Count <= 100)
            {
                pendingEvents.Add(new PendingEvent(name, clonedEventParameters));
            }
            if (logEventReporter == null)
            {
                return;
            }
            logEventReporter.Logger.LogInformation($"Log event: {name}");
            Dictionary<string, Dictionary<string, string>> events = new Dictionary<string, Dictionary<string, string>>
            {
                { name, clonedEventParameters }
            };
            var orderedIntercepts = logEventReporter.intercepts.OrderBy(i => i.Order);
            foreach (var intercept in orderedIntercepts)
            {
                var cloneEvents = new Dictionary<string, Dictionary<string, string>>(events);
                foreach (var @event in cloneEvents)
                {
                    var interceptEvents = await intercept.Execute(@event.Key, @event.Value);
                    foreach (var interceptEvent in interceptEvents)
                    {
                        if (!events.ContainsKey(interceptEvent.Key))
                        {
                            events.Add(interceptEvent.Key, interceptEvent.Value);
                        }
                        else
                        {
                            var @eventParameter = events[interceptEvent.Key];
                            foreach (var parameter in interceptEvent.Value)
                            {
                                if (@eventParameter.ContainsKey(parameter.Key))
                                {
                                    continue;
                                }
                                @eventParameter.Add(parameter.Key, parameter.Value);
                            }
                        }
                    }
                }
            }
            foreach (var eventReporter in logEventReporter.eventReporters)
            {
                foreach (var @event in events)
                {
                    try
                    {
                        await eventReporter.LogEvent(@event.Key, @event.Value);
                    }
                    catch (Exception ex)
                    {
                        logEventReporter.Logger.LogError(ex.Message);
                    }
                }
            }
        }
    }
    internal class PendingEvent
    {
        public PendingEvent(string eventName, Dictionary<string, string> eventParameters)
        {
            EventName = eventName;
            EventParameters = eventParameters;
        }
        public string EventName { get; set; }
        public Dictionary<string, string> EventParameters { get; set; }
    }
    */
}
