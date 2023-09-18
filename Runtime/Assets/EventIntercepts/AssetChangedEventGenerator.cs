using SuuchaStudio.Unity.Core.LogEvents;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SuuchaStudio.Unity.Core.Assets.EventIntercepts
{
    public class AssetChangedEventGenerator: SuuchaBase
    {
        private readonly List<AssetChangedEventConfiguration> assetChangedEventConfigurations;
        private readonly Dictionary<string, int> maxDigits = new Dictionary<string, int>();
        /// <summary>
        /// Initializes a new instance of the <see cref="AssetChangedEventGenerator"/> class with the provided asset change event configurations.
        /// </summary>
        /// <param name="assetChangedEventConfigurations">The list of asset change event configurations.</param>
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
        /// <summary>
        /// Handles asset increased callback by checking configurations and triggering corresponding events.
        /// </summary>
        /// <param name="code">The asset code.</param>
        /// <param name="newValue">The new asset value.</param>
        /// <param name="oldValue">The old asset value.</param>
        public async void AssetIncreasedCallback(string code, long newValue, long oldValue)
        {
            var config = assetChangedEventConfigurations.FirstOrDefault(a => a.AssetCode == code);
            if (config == null)
            {
                Logger.LogInformation($"Asset configuration not found for asset code: {code}");
                return;
            }
            var targets = config.Values.Where(v => v > oldValue && v <= newValue);
            foreach (var target in targets)
            {
                var value = CountString(code, target);
                await Suucha.App.LogEvent($"{config.EventName}_{value}", new Dictionary<string, string>());
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
        /// Gets or sets the asset code.
        /// </summary>
        /// <remarks>
        /// The asset code uniquely identifies the asset being monitored for changes.
        /// </remarks>
        [JsonProperty("assetCode")]
        public string AssetCode { get; set; }
        /// <summary>
        /// Gets or sets the event name.
        /// </summary>
        /// <remarks>
        /// The event name describes the type of change event associated with the asset.
        /// </remarks>
        [JsonProperty("eventName")]
        public string EventName { get; set; }
        /// <summary>
        /// Gets or sets the list of monitored asset values.
        /// </summary>
        /// <remarks>
        /// When the asset undergoes a change from a lower value to a value in this list,
        /// one or more asset change events will be reported.
        /// Example: "gold_coin_200" indicates that the gold coin count has reached or exceeded 200.
        /// </remarks>
        [JsonProperty("values")]
        public List<long> Values { get; set; }
    }
}
