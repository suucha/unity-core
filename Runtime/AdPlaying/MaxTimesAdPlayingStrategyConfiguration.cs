using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SuuchaStudio.Unity.Core.AdPlaying
{
    /// <summary>
    /// 每天最多播放次数广告播放策略配置
    /// </summary>
    public class MaxTimesAdPlayingStrategyConfiguration
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MaxTimesAdPlayingStrategyConfiguration()
        {
            AdUnits = new List<MaxTimesAdUnitConfiguration>();
        }
        /// <summary>
        /// 最大播放次数
        /// </summary>
        /// <remarks>
        /// 激励视频和插屏广告总播放最大次数。
        /// </remarks>
        [JsonProperty("maxTimes")]
        public int MaxTimes { get; set; }
        /// <summary>
        /// 激励视频最大播放次数
        /// </summary>
        [JsonProperty("rewardedMaxTimes")]
        public int RewardedMaxTimes { get; set; }
        /// <summary>
        /// 插屏最大播放次数
        /// </summary>
        [JsonProperty("interstitialMaxTimes")]
        public int InterstitialMaxTimes { get; set; }
        /// <summary>
        /// 广告单元配置列表
        /// </summary>
        [JsonProperty("adUnits")]
        public List<MaxTimesAdUnitConfiguration> AdUnits { get; set; }
        /// <summary>
        /// 广告获取失败重试时间间隔，单位：毫秒
        /// </summary>
        /// <example>
        /// 如果配置为：5000,5000,10000,15000，则表示：
        /// 如果第一次广告获取失败，那么5秒中后再次重试，如果再失败，再间隔5秒后重试，之后间隔10秒，间隔15秒，一共重试4次
        /// </example>
        [JsonProperty("retryIntervals")]
        public List<int> RetryIntervals { get; set; }
        /// <summary>
        /// 根据广告单元ID获取最大播放次数
        /// </summary>
        /// <param name="adUnitId">广告单元ID</param>
        /// <returns></returns>
        public int GetMaxTimes(string adUnitId)
        {
            var adUnit = AdUnits.FirstOrDefault(a => a.AdUnitId == adUnitId);
            if (adUnit == null)
            {
                return 0;
            }
            return adUnit.MaxTimesToPlay;
        }
    }
    /// <summary>
    /// 广告单元最大播放次数配置
    /// </summary>
    public class MaxTimesAdUnitConfiguration
    {
        /// <summary>
        /// 广告类型
        /// </summary>
        [JsonProperty("adType")]
        public AdType AdType { get; set; }
        /// <summary>
        /// 广告单元ID
        /// </summary>
        [JsonProperty("adUnitId")]
        public string AdUnitId { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        [JsonProperty("priority")]
        public int Priority { get; set; }
        /// <summary>
        /// 最大播放次数
        /// </summary>
        [JsonProperty("maxTimesToPlay")]
        public int MaxTimesToPlay { get; set; }
    }
}
