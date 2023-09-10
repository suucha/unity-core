using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace SuuchaStudio.Unity.Core.LogEvents.Intercepts
{
    /// <summary>
    /// 这是一个全局拦截器
    /// </summary>
    public class EventCountParameterIntercept : SuuchaBase, ILogEventParameterIntercept
    {
        private readonly string fileName = "suuchaEventReportCount.db";
        private readonly string filePath;
        private readonly Dictionary<string, int> counts = new Dictionary<string, int>();
        private readonly Dictionary<string, int> currentCounts = new Dictionary<string, int>();
        public EventCountParameterIntercept(string filePath)
        {
            this.filePath = filePath;
            counts = GetDb();
        }
        private Dictionary<string, int> GetDb()
        {
            var file = Path.Combine(filePath, fileName);
            if (!File.Exists(file))
            {
                File.WriteAllText(file, "{}");
            }
            var content = File.ReadAllText(file);
            var oldData = new Dictionary<string, int>();
            try
            {
                oldData = JsonConvert.DeserializeObject<Dictionary<string, int>>(content);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Get data from file {file} error: {ex.Message}");
                return oldData;
            }
            return oldData;
        }
        private void SaveDb()
        {
            var file = Path.Combine(filePath, fileName);
            if (!File.Exists(file))
            {
                File.WriteAllText(file, "{}");
            }
            var json = JsonConvert.SerializeObject(counts);
            File.WriteAllText(file, json);
        }
        public string Name => "EventReportCount";

        public int Order => 100;

        public UniTask<Dictionary<string, string>> Execute(string eventName, Dictionary<string, string> parameters)
        {
            var newParameters = new Dictionary<string, string>();
            var count = 0;
            if (!counts.ContainsKey(eventName))
            {
                counts.Add(eventName, count);
            }
            count = counts[eventName];
            count++;
            counts[eventName] = count;
            SaveDb();
            newParameters.Add(EventParameterNames.EventCumulativeCount, count.ToString());
            count = 0;
            if (!currentCounts.ContainsKey(eventName))
            {
                currentCounts.Add(eventName, count);
            }
            count = currentCounts[eventName];
            count++;
            currentCounts[eventName] = count;
            newParameters.Add(EventParameterNames.EventCount, count.ToString());
            return UniTask.FromResult(newParameters);
        }
    }
}
