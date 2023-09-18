namespace SuuchaStudio.Unity.Core.AdPlaying
{
    /// <summary>
    /// 广告播放策略
    /// </summary>
    public interface IAdPlayingStrategy
    {
        /// <summary>
        /// 策略名称
        /// </summary>
        string StrategyName { get; }
        /// <summary>
        /// 根据广告位置获取激励视频广告单元ID
        /// </summary>
        /// <param name="placement">广告位置</param>
        /// <param name="adRequestResults">获取结果</param>
        /// <returns>如果有对应的广告单元，则返回广告单元ID，否则返回空，同时adGetResults返回原因</returns>
        string GetRewardedVideoAdUnitId(string placement, out AdRequestResults adRequestResults);
        /// <summary>
        /// 根据广告位置获取插屏广告单元ID
        /// </summary>
        /// <param name="placement">广告位置</param>
        /// <param name="adRequestResults">获取结果</param>
        /// <returns>如果有对应的广告单元，则返回广告单元ID，否则返回空，同时adGetResults返回原因</returns>
        string GetInterstitialVideoAdUnitId(string placement, out AdRequestResults adRequestResults);
        /// <summary>
        /// 根据广告位置获取Banner广告单元ID
        /// </summary>
        /// <param name="placement">广告位置</param>
        /// <param name="adRequestResults">获取结果</param>
        /// <returns>如果有对应的广告单元，则返回广告单元ID，否则返回空，同时adGetResults返回原因</returns>
        string GetBannerAdUnitId(string placement, out AdRequestResults adRequestResults);
    }
}
