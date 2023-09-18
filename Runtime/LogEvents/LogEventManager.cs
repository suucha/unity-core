using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace SuuchaStudio.Unity.Core.LogEvents
{
    internal class LogEventManager : SuuchaBase
    {
        private const int DefaultCacheSize = 100;
        private static readonly Queue<LogEvent> eventCache = new Queue<LogEvent>(DefaultCacheSize);
        private static readonly List<ILogEventReporter> reporters = new List<ILogEventReporter>();
        private static readonly List<ILogEventIntercept> eventInterceptors = new List<ILogEventIntercept>();
        private static readonly List<ILogEventParameterIntercept> eventParameterInterceptors = new List<ILogEventParameterIntercept>();

        private static readonly LogEventManager instance;
        static LogEventManager() {
            instance = new LogEventManager();
        }
        /// <summary>
        /// Adds an <see cref="ILogEventReporter"/> to the list of reporters.
        /// </summary>
        /// <param name="reporter">The reporter to add.</param>
        /// <remarks>
        /// This method checks if a reporter with the same name and type already exists in the list of reporters.
        /// If a matching reporter is found, the new reporter is not added.
        /// Otherwise, the new reporter is added to the list of reporters.
        /// Cached log events are reported to the new reporter after it is added.
        /// </remarks>
        /// <param name="reporter">The reporter to add.</param>
        public static async UniTask AddReporter(ILogEventReporter reporter)
        {
            // Check if a reporter with the same name already exists.
            if (reporters.Any(r => r.Name == reporter.Name
              && reporter.GetType().FullName == r.GetType().FullName))
            {
                return;
            }

            reporters.Add(reporter);

            // Report cached events to the new reporter.
            foreach (var logEvent in eventCache)
            {
                await ReportLogEvent(reporter, logEvent);
            }
        }

        /// <summary>
        /// Reports a log event to an <see cref="ILogEventReporter"/> and its intercepted events if enabled.
        /// </summary>
        /// <param name="reporter">The reporter to which the log event is reported.</param>
        /// <param name="logEvent">The log event to report.</param>
        private static async UniTask ReportLogEvent(ILogEventReporter reporter, LogEvent logEvent)
        {
            await reporter.LogEvent(logEvent.Name, logEvent.Parameters);
            var reportedEventNames = new List<string> { logEvent.Name };
            foreach (var newLogEventName in logEvent.NewNames)
            {
                if (reporter.IsEnableEventIntercept(newLogEventName.InterceptorName))
                {
                    if(reportedEventNames.Contains(newLogEventName.Name))
                    {
                        await reporter.LogEvent(newLogEventName.Name, logEvent.Parameters);
                        reportedEventNames.Add(newLogEventName.Name);
                    }
                }
                else
                {
                    instance.Logger.LogInformation($"Event interceptor '{newLogEventName.InterceptorName}' is not enabled, skipping event(name: {newLogEventName.Name}) reporting.");
                }

            }
        }

        /// <summary>
        /// Logs a custom event with optional parameters and applies interceptors before reporting.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        /// <param name="parameters">Optional event parameters as key-value pairs.</param>
        public static async UniTask LogEvent(string name, Dictionary<string, string> parameters = null)
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }
            instance.Logger.LogDebug($"LogEvent started for event: {name}, parameters: {JsonConvert.SerializeObject(parameters)}");
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
                    instance.Logger.LogDebug($"New event parameter added - Name: {parameter.Key}, Value: {parameter.Value}, Interceptor: {parameterIntercept.Name}");
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
                        instance.Logger.LogDebug($"New event name: {logEventName}, added by event interceptor: {interceptor.Name}");
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

        /// <summary>
        /// Adds an event interceptor to the list of interceptors, ensuring it's not null and not a duplicate.
        /// </summary>
        /// <param name="interceptor">The event interceptor to add.</param>
        public static void AddEventInterceptor(ILogEventIntercept interceptor)
        {
            instance.Logger.LogDebug($"Adding event interceptor '{interceptor?.Name}'");
            if (interceptor == null 
                || eventInterceptors.FirstOrDefault(e=>e.Name == interceptor.Name) != null)
            {
                instance.Logger.LogWarning($"Event interceptor '{interceptor?.Name}' is null or a duplicate and will not be added.");
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
            instance.Logger.LogDebug($"Event interceptor '{interceptor?.Name}' added successfully.");
        }
    }


}
