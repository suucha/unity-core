using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace SuuchaStudio.Unity.Core.LogEvents.Intercepts
{
    /// <summary>
    /// 这个是一个全局拦截器，只能有一个实例，各事件上报器只需要配置需要拦截的事件即可
    /// </summary>
    public class EventCounterIntercept : SuuchaBase, ILogEventIntercept
    {
        private readonly string EventCounterStorageKey = "SuuchaEventCounterStorageKey";
        private readonly List<EventCounterConfiguration> eventCounterConfigurations;
        private readonly Dictionary<string, int> maxDigits = new Dictionary<string, int>();
        public EventCounterIntercept(List<EventCounterConfiguration> eventCounterConfigurations)
        {
            this.eventCounterConfigurations = eventCounterConfigurations;
            if (this.eventCounterConfigurations == null)
            {
                this.eventCounterConfigurations = new List<EventCounterConfiguration>();
            }
            foreach (var ecc in this.eventCounterConfigurations)
            {
                if (string.IsNullOrEmpty(ecc.Name) || ecc.CountList == null || ecc.CountList.Count == 0)
                {
                    continue;
                }
                var maxCount = ecc.CountList.Max();
                var maxLength = maxCount.ToString().Length;
                if (maxDigits.ContainsKey(ecc.Name))
                {
                    var oldLength = maxDigits[ecc.Name];
                    if (oldLength > maxLength)
                    {
                        maxLength = oldLength;
                    }
                    maxDigits.Remove(ecc.Name);
                }
                maxDigits.Add(ecc.Name, maxLength);
                Logger.LogDebug($"event name:{ecc.Name}, max length:{maxLength}");
            }
        }
        public string Name => "EventCounterIntercept";
        public int Order => 100;

        public UniTask<List<string>> Execute(string eventName, Dictionary<string, string> parameters)
        {
            var events = new List<string> { eventName };
            if (eventCounterConfigurations.Count == 0)
            {
                return UniTask.FromResult(events);
            }
            var needCountEvent = eventCounterConfigurations.FirstOrDefault(ec =>
            (ec.Name.EndsWith("_") && (eventName.StartsWith(ec.Name) || eventName == ec.Name.Substring(0, ec.Name.Length - 1)) ||
            (!ec.Name.EndsWith("_") && eventName == ec.Name)));
            if (needCountEvent == null)
            {
                return UniTask.FromResult(events);
            }
            var eventCountList = LocalStorage.Get<List<EventCounterDao>>(EventCounterStorageKey);
            if (eventCountList == null)
            {
                eventCountList = new List<EventCounterDao>();
            }
            var eventCount = eventCountList.FirstOrDefault(ec => ec.Name == eventName);
            if (eventCount == null)
            {
                eventCount = new EventCounterDao
                {
                    Name = eventName
                };
                eventCountList.Add(eventCount);
            }
            eventCount.Count += 1;
            LocalStorage.Set(EventCounterStorageKey, eventCountList);
            LocalStorage.Save();
            if (needCountEvent.CountList.Contains(eventCount.Count))
            {
                var counter = CountString(needCountEvent.Name, eventCount.Count);
                events.Add($"{eventName}_{counter}");
            }
            return UniTask.FromResult(events);
        }
        private string CountString(string eventName, int count)
        {
            if (!maxDigits.ContainsKey(eventName))
            {
                return count.ToString();
            }
            var digits = maxDigits[eventName];
            var countString = count.ToString();
            return countString.PadLeft(digits, '0');
        }
    }

    public class EventCounterConfiguration
    {
        public EventCounterConfiguration()
        {
            CountList = new List<int>();
        }
        /// <summary>
        /// 需要计数的事件名称，
        /// 如果名称以_结束，则表示所有以此开始的事件都需要计数
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// 需要计数个数的列表
        /// </summary>
        [JsonProperty("countList")]
        public List<int> CountList { get; set; }
    }
    internal class EventCounterDao
    {
        public EventCounterDao()
        {
            Count = 0;
        }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
    }

}
