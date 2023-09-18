using System.Linq;
using System.Collections.Generic;
using System;
using SuuchaStudio.Unity.Core.Utils;
using Cysharp.Threading.Tasks;

namespace SuuchaStudio.Unity.Core.LogEvents.Intercepts
{
    public class AbGroupEventIntercept : SuuchaBase, ILogEventIntercept
    {
        private readonly List<AbGroupEvent> abGroupEvents;
        public string Name => "AbGroupEventIntercept";
        public int Order => 50;
        private readonly Func<string, string> getGroupValue;
        public AbGroupEventIntercept(List<AbGroupEvent> abGroupEvents, Func<string, string> getGroupValue)
        {
            this.abGroupEvents = abGroupEvents ?? new List<AbGroupEvent>();
            this.getGroupValue = getGroupValue;
        }
        public UniTask<List<string>> Execute(string eventName, Dictionary<string, string> parameters)
        {
            var events = new List<string> { eventName };
            if (abGroupEvents.Count == 0)
            {
                return UniTask.FromResult(events);
            }
            var groups = abGroupEvents.Where(ab => ab.EventNames.MatchUnderscore(eventName));
            foreach (var group in groups)
            {
                var value = "";
                if (getGroupValue != null)
                {
                    value = getGroupValue(group.Name);
                }
                if (string.IsNullOrEmpty(value))
                {
                    events.Add($"{eventName}_{group.Name}");
                }
                else
                {
                    events.Add($"{eventName}_{group.Name}_{value}");
                }
            }
            return UniTask.FromResult(events);
        }
    }
}
