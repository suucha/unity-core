using SuuchaStudio.Unity.Core.LogEvents;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SuuchaStudio.Unity.Core.Assets.EventIntercepts
{
    public class AssetChangedEventGenerator
    {
        private readonly List<AssetChangedEventConfiguration> assetChangedEventConfigurations;
        private readonly Dictionary<string, int> maxDigits = new Dictionary<string, int>();
        public AssetChangedEventGenerator(List<AssetChangedEventConfiguration> assetChangedEventConfigurations)
        {
            this.assetChangedEventConfigurations = assetChangedEventConfigurations;
            if (this.assetChangedEventConfigurations == null)
            {
                this.assetChangedEventConfigurations = new List<AssetChangedEventConfiguration>();
            }
            foreach (var ace in this.assetChangedEventConfigurations)
            {
                if (ace.Values.Count == 0)
                {
                    continue;
                }
                var maxValue = ace.Values.Max();
                var maxLength = maxValue.ToString().Length;
                if (maxDigits.ContainsKey(ace.AssetCode))
                {
                    var oldLength = maxDigits[ace.AssetCode];
                    if (oldLength > maxLength)
                    {
                        maxLength = oldLength;
                    }
                    maxDigits.Remove(ace.AssetCode);
                }
                maxDigits.Add(ace.AssetCode, maxLength);
            }
        }
        public async void AssetIncreasedCallback(string code, long newValue, long oldValue)
        {
            var config = assetChangedEventConfigurations.FirstOrDefault(a => a.AssetCode == code);
            if (config == null)
            {
                return;
            }
            var targets = config.Values.Where(v => v > oldValue && v <= newValue);
            foreach (var target in targets)
            {
                var value = CountString(code, target);
                await LogEventManager.LogEvent($"{config.EventName}_{value}", new Dictionary<string, string>());
            }

        }
        private string CountString(string code, long value)
        {
            if (!maxDigits.ContainsKey(code))
            {
                return value.ToString();
            }
            var digits = maxDigits[code];
            var countString = value.ToString();
            return countString.PadLeft(digits, '0');
        }
#pragma warning disable IDE0060 // 删除未使用的参数
        public void AssetReducedCallback(string code, long newValue, long oldValue)
#pragma warning restore IDE0060 // 删除未使用的参数
        {

        }
    }

    public class AssetChangedEventConfiguration
    {
        /// <summary>
        /// 资产编码
        /// </summary>
        [JsonProperty("assetCode")]
        public string AssetCode { get; set; }
        /// <summary>
        /// 事件名称
        /// </summary>
        [JsonProperty("eventName")]
        public string EventName { get; set; }
        /// <summary>
        /// 监听的资产值列表
        /// </summary>
        /// <remarks>
        /// 当某资产某次由低到高的变化中包含此列表中的值，则上报一个或多个资产变化事件
        /// 如：gold_coin_200，表示金币已达到200或超过200
        /// </remarks>
        [JsonProperty("values")]
        public List<long> Values { get; set; }
    }
}
