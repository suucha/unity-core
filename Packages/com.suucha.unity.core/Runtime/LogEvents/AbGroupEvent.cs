using Newtonsoft.Json;
using System.Collections.Generic;

namespace SuuchaStudio.Unity.Core.LogEvents
{
    public class AbGroupEvent
    {
        /// <summary>
        /// AB分组的名称
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        /// <summary>
        /// 需要区分AB分组的事件名称列表
        /// </summary>
        [JsonProperty("eventNames")]
        public List<string> EventNames { get; set; }
        public AbGroupEvent()
        {
            EventNames = new List<string>();
        }

    }
}
