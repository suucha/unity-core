using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using SuuchaStudio.Unity.Core.Utils;

namespace SuuchaStudio.Unity.Core.LogEvents
{
    public abstract class LogEventReporterAbstract : SuuchaBase, ILogEventReporter
    {
        public abstract string Name { get; }

        public List<string> AllowedEventNames { get; private set; }

        public List<string> ExcludedEventNames { get; private set; }

        public Dictionary<string, string> EventNameMap { get; private set; }

        public Dictionary<string, string> EventParameterNameMap { get; private set; }

        public Dictionary<string, string> RequiredEventParameters { get; private set; }
        private List<string> requiredEventParameterNames;
        private int requiredEventParametersTimeout = 30000;
        protected List<string> AddedRequiredEventParameterNames { get; private set; }


        public Dictionary<string, string> CommonEventParameters { get; private set; }
        public List<string> EnabledEventInterceptTypeNames { get; private set; }
        private readonly List<CachableLogEvent> cachableLogEvents;

        public LogEventReporterAbstract()
        {
            AllowedEventNames = new List<string>();
            ExcludedEventNames = new List<string>();
            EventNameMap = new Dictionary<string, string>();
            EventParameterNameMap = new Dictionary<string, string>();
            RequiredEventParameters = new Dictionary<string, string>();
            requiredEventParameterNames = new List<string>();
            CommonEventParameters = new Dictionary<string, string>();
            AddedRequiredEventParameterNames = new List<string>();
            EnabledEventInterceptTypeNames = new List<string>();
            cachableLogEvents = new List<CachableLogEvent>();

        }
        public LogEventReporterAbstract(List<string> allowedEventNames,
            List<string> excludedEventNames,
            Dictionary<string, string> eventNameMap,
            Dictionary<string, string> eventParameterNameMap)
            : this()
        {
            AllowedEventNames = allowedEventNames ?? new List<string>();
            ExcludedEventNames = excludedEventNames ?? new List<string>();
            EventNameMap = eventNameMap ?? new Dictionary<string, string>();
            EventParameterNameMap = eventParameterNameMap ?? new Dictionary<string, string>();
        }

        public bool IsAllowEventReport(string eventName)
        {
            //如果允许和排除事件名称列表都为空，表示所有事件都要上报
            if ((AllowedEventNames == null || AllowedEventNames.Count == 0)
                && (ExcludedEventNames == null || ExcludedEventNames.Count == 0))
            {
                return true;
            }
            //如果排除事件名称列表为空则判断允许事件名称列表中是否有该事件
            if ((ExcludedEventNames == null || ExcludedEventNames.Count == 0))
            {
                return AllowedEventNames.MatchUnderscore(eventName);
            }
            return !ExcludedEventNames.MatchUnderscore(eventName);
        }


        public void ChangeAllowedEventNames(List<string> eventNames)
        {
            AllowedEventNames = eventNames ?? new List<string>();
        }

        public void ChangeExcludedEventNames(List<string> eventNames)
        {
            ExcludedEventNames = eventNames ?? new List<string>();
        }

        public void ChangeEventNameMap(Dictionary<string, string> nameMap)
        {
            EventNameMap = nameMap ?? new Dictionary<string, string>();

        }

        public void ChangeEventParameterNameMap(Dictionary<string, string> nameMap)
        {
            EventParameterNameMap = nameMap ?? new Dictionary<string, string>();
        }

        public void AddCommonEventParameter(string eventParameterName, string value)
        {
            if (CommonEventParameters.ContainsKey(eventParameterName))
            {
                CommonEventParameters[eventParameterName] = value;
            }
            else
            {
                CommonEventParameters.Add(eventParameterName, value);
            }
            var mapParameterName = MapName(eventParameterName, EventParameterNameMap);
            foreach (var @event in cachableLogEvents)
            {
                if (@event.Parameters.ContainsKey(mapParameterName))
                {
                    @event.Parameters[mapParameterName] = value;
                }
                else
                {
                    @event.Parameters.Add(mapParameterName, value);
                }
            }
            if (requiredEventParameterNames.Contains(eventParameterName)
                && !AddedRequiredEventParameterNames.Contains(eventParameterName))
            {
                AddedRequiredEventParameterNames.Add(eventParameterName);
                if (IsAllRequiredEventParametersAdded)
                {
                    ReportCachedEvents().GetAwaiter().GetResult();
                }
            }

        }

        private async UniTask ReportCachedEvents()
        {
            foreach (var @event in cachableLogEvents.OrderBy(e => e.Timestamp))
            {
                await LogEventInternal(@event.Name, @event.Parameters);
            }
            cachableLogEvents.Clear();
        }


        public bool IsEnableEventIntercept(string typeName)
        {
            return EnabledEventInterceptTypeNames.Contains(typeName);
        }

        public UniTask LogEvent(string name, Dictionary<string, string> eventParameters)
        {
            if (!IsAllowEventReport(name))
            {
                return UniTask.CompletedTask;
            }
            var cloneEventParameters = new Dictionary<string, string>();
            foreach (var commonParameter in CommonEventParameters)
            {
                cloneEventParameters.Add(commonParameter.Key, commonParameter.Value);
            }
            foreach (var parameter in eventParameters)
            {
                if (!cloneEventParameters.ContainsKey(parameter.Key))
                {
                    cloneEventParameters.Add(parameter.Key, parameter.Value);
                }
                else
                {
                    //上报事件中带的事件参数将会覆盖掉公共事件参数中同名的参数
                    cloneEventParameters[parameter.Key] = parameter.Value;
                }
            }
            var eventMapName = MapName(name, EventNameMap);
            var mapEventParameters = new Dictionary<string, string>();
            foreach (var parameter in cloneEventParameters)
            {
                var mapParameterName = MapName(parameter.Key, EventParameterNameMap);
                mapEventParameters.Add(mapParameterName, parameter.Value);
            }
            if (!IsAllRequiredEventParametersAdded)
            {
                cachableLogEvents.Add(new CachableLogEvent
                {
                    Name = eventMapName,
                    Parameters = mapEventParameters
                });
                return UniTask.CompletedTask;
            }
            return LogEventInternal(eventMapName, mapEventParameters);
        }
        protected abstract UniTask LogEventInternal(string name, Dictionary<string, string> eventParameters);

        #region Required Event Parameters
        private UniTask OnRequiredEventParametersTimeout()
        {
            return ReportCachedEvents();
        }
        protected bool IsAllRequiredEventParametersAdded
        {
            get
            {
                return AddedRequiredEventParameterNames.Count >= requiredEventParameterNames.Count;
            }
        }
        public void AddRequiredParameters(Dictionary<string, string> requiredEventParameters, int timeout = 30000)
        {
            RequiredEventParameters = requiredEventParameters ?? new Dictionary<string, string>();
            requiredEventParameterNames = RequiredEventParameters.Keys.ToList();
            requiredEventParametersTimeout = timeout;
            if (requiredEventParameterNames.Count > 0)
            {
                Suucha.App.AddTask(requiredEventParametersTimeout, requiredEventParametersTimeout, 1, OnRequiredEventParametersTimeout);
            }
        }
        #endregion
        protected string MapName(string eventName, Dictionary<string, string> nameMap)
        {
            if (nameMap == null || nameMap.Count == 0)
            {
                return eventName;
            }
            var needMapName = nameMap.Keys.MatchUnderscoreFirstOrDefault(eventName);
            if (string.IsNullOrEmpty(needMapName))
            {
                return eventName;
            }
            var mapName = nameMap[needMapName];
            if (!needMapName.EndsWith("_"))
            {
                return mapName;
            }
            needMapName = needMapName.Substring(0, needMapName.Length - 1);
            if (needMapName == eventName)
            {
                return mapName;
            }
            mapName = eventName.Replace($"{needMapName}_", $"{mapName}_");
            if (mapName.EndsWith("_"))
            {
                mapName = mapName.Substring(0, mapName.Length - 1);
            }
            return mapName;
        }

        public void EnableEventIntercept<TEventIntercept>() where TEventIntercept : ILogEventIntercept
        {
            var type = typeof(TEventIntercept);
            var typeName = $"{type.Namespace}.{type.Name}";
            if (!EnabledEventInterceptTypeNames.Contains(typeName))
            {
                EnabledEventInterceptTypeNames.Add(typeName);
            }
        }
        protected class CachableLogEvent
        {
            public string Name { get; set; }
            public Dictionary<string, string> Parameters { get; set; }
            public long Timestamp { get; private set; }
            public CachableLogEvent()
            {
                Parameters = new Dictionary<string, string>();
                Timestamp = DateTime.Now.Ticks;
            }
        }
    }
    /*
    /// 
    /// <summary>
    /// 理想的使用方式：
    /// LogEventReporter.Init(commonEventParameters)
    /// .EnableAbGroup(abGroups, abGroupEventNames)
    /// .UseAppsflyer(appsflyerEventNameMap, appsflyerEventParameterNameMap)
    /// .UseFirebase(firebaseEventNameMap, firebaseEventParameterNameMap);
    /// LogEventReporter.LogEvent(name, parameters);
    /// </summary>
    public abstract class LogEventReporterAbstract : SuuchaBase, ILogEventReporter
    {
        public abstract string Name { get; }
        protected List<string> enabledEventNames;
        protected Dictionary<string, string> commonEventParameters;
        protected Dictionary<string, string> eventNameMap;
        protected Dictionary<string, string> eventParameterNameMap;
        protected List<string> requiredEventParameterNames;
        private readonly List<ILogEventIntercept> eventIntercepts;
        private readonly List<ILogEventParameterIntercept> eventParameterIntercepts;

        protected LogEventReporterAbstract()
        {
            commonEventParameters = new Dictionary<string, string>();
            requiredEventParameterNames = new List<string>();
            eventIntercepts = new List<ILogEventIntercept>();
            eventParameterIntercepts = new List<ILogEventParameterIntercept>();
        }

        public UniTask Initialize(List<string> enabledEventNames,
            Dictionary<string, string> eventNameMap,
            Dictionary<string, string> eventParameterNameMap,
            List<string> requiredEventParameterNames)
        {
            Logger.LogInformation($"Log event reporter {Name} init start.");
            this.eventNameMap = eventNameMap;
            if (this.eventNameMap == null)
            {
                this.eventNameMap = new Dictionary<string, string>();
            }
            this.eventParameterNameMap = eventParameterNameMap;
            if (this.eventParameterNameMap == null)
            {
                this.eventParameterNameMap = new Dictionary<string, string>();
            }
            this.enabledEventNames = enabledEventNames;
            if (this.enabledEventNames == null)
            {
                this.enabledEventNames = new List<string>();
            }
            this.requiredEventParameterNames = requiredEventParameterNames;
            if (this.requiredEventParameterNames == null)
            {
                this.requiredEventParameterNames = new List<string>();
            }
            Logger.LogInformation($"Log event reporter {Name} init finished.");
            AfterInitialize(this.enabledEventNames, this.eventNameMap, this.eventParameterNameMap, this.requiredEventParameterNames);
            return UniTask.CompletedTask;
        }
        protected virtual void AfterInitialize(List<string> enabledEventNames,
            Dictionary<string, string> eventNameMap,
            Dictionary<string, string> eventParameterNameMap,
            List<string> requiredEventParameterNames)
        {

        }
        public void AddCommonEventParameter(string eventParameterName, string value)
        {
            if (commonEventParameters.ContainsKey(eventParameterName))
            {
                commonEventParameters.Remove(eventParameterName);
            }
            commonEventParameters.Add(eventParameterName, value);
            AfterAddCommonEventParameter(eventParameterName, value);
        }
        protected virtual void AfterAddCommonEventParameter(string eventParameterName, string value)
        {

        }
        public UniTask LogEvent(string name, Dictionary<string, string> eventParameters)
        {
            Logger.LogInformation($"events: {Newtonsoft.Json.JsonConvert.SerializeObject(enabledEventNames)}, event name: {name}");
            if (enabledEventNames.Count > 0)
            {
                var needReportEvent = enabledEventNames.MatchUnderscore(name);
                if (!needReportEvent)
                {
                    return UniTask.CompletedTask;
                }
            }
            Logger.LogInformation($"Log event to: {Name}, event name: {name}");
            var cloneEventParameters = new Dictionary<string, string>();
            foreach (var commonParameter in commonEventParameters)
            {
                cloneEventParameters.Add(commonParameter.Key, commonParameter.Value);
            }
            foreach (var parameter in eventParameters)
            {
                if (!cloneEventParameters.ContainsKey(parameter.Key))
                {
                    cloneEventParameters.Add(parameter.Key, parameter.Value);
                }
                else
                {
                    cloneEventParameters[parameter.Key] = parameter.Value;
                }
            }
            var mapName = MapName(name, eventNameMap);
            var mapEventParameters = new Dictionary<string, string>();
            foreach (var parameter in cloneEventParameters)
            {
                var mapParameterName = MapName(parameter.Key, eventParameterNameMap);
                mapEventParameters.Add(mapParameterName, parameter.Value);
            }
            return LogEventInternal(mapName, mapEventParameters);
        }
        protected string MapName(string eventName, Dictionary<string, string> nameMap)
        {
            if (nameMap == null || nameMap.Count == 0)
            {
                return eventName;
            }
            var needMapName = nameMap.Keys.MatchUnderscoreFirstOrDefault(eventName);
            if (string.IsNullOrEmpty(needMapName))
            {
                return eventName;
            }
            var mapName = nameMap[needMapName];
            if (!needMapName.EndsWith("_"))
            {
                return mapName;
            }
            needMapName = needMapName.Substring(0, needMapName.Length - 1);
            if (needMapName == eventName)
            {
                return mapName;
            }
            mapName = eventName.Replace($"{needMapName}_", $"{mapName}_");
            if (mapName.EndsWith("_"))
            {
                mapName = mapName.Substring(0, mapName.Length - 1);
            }
            return mapName;
        }
        protected abstract UniTask LogEventInternal(string name, Dictionary<string, string> eventParameters);

        public void ChangeEnabledEventNames(params string[] eventNames)
        {
            enabledEventNames = new List<string>(eventNames);
        }

        public void ChangeEventNameMap(Dictionary<string, string> eventNameMap)
        {
            this.eventNameMap = eventNameMap;
            if (this.eventNameMap == null)
            {
                this.eventNameMap = new Dictionary<string, string>();
            }
        }

        public void ChangeEventParameterNameMap(Dictionary<string, string> eventParameterNameMap)
        {
            this.eventParameterNameMap = eventParameterNameMap;
            if (this.eventParameterNameMap == null)
            {
                this.eventParameterNameMap = new Dictionary<string, string>();
            }
        }
    }
    */
}
