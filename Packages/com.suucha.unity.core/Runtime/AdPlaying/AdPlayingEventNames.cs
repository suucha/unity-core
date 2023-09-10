namespace SuuchaStudio.Unity.Core.AdPlaying
{
    public class AdPlayingEventNames
    {
        /// <summary>
        /// 广告收益事件参数名称，默认值为"ad_revenue"
        /// </summary>
        /// <remarks>
        /// 不能为空，如果为空则使用默认值
        /// </remarks>
        public static string RevenuePaidEventParameterName { get; set; } = "ad_revenue";
        #region Rewarded video
        /// <summary>
        /// 激励视频广告加载成功事件名称，默认值为"ad_rewarded_Loaded"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string RewardedVideoLoaded { get; set; } = "ad_rewarded_loaded";
        /// <summary>
        /// 激励视频广告点击事件名称，默认值为"ad_rewarded_clicked"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string RewardedVideoClicked { get; set; } = "ad_rewarded_clicked";
        /// <summary>
        /// 激励视频广告展示完成事件名称，默认值为"ad_rewarded_shown"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string RewardedVideoShown { get; set; } = "ad_rewarded_shown";
        /// <summary>
        /// 激励视频广告关闭事件名称，默认值为"ad_rewarded_closed"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string RewardedVideoClosed { get; set; } = "ad_rewarded_closed";
        /// <summary>
        /// 激励视频广告加载失败事件名称，默认值为"ad_rewarded_load_failed"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string RewardedVideoLoadFailed { get; set; } = "ad_rewarded_load_failed";
        /// <summary>
        /// 激励视频广告展示失败事件名称，默认值为"ad_rewarded_show_failed"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string RewardedVideoShowFailed { get; set; } = "ad_rewarded_show_failed";
        /// <summary>
        /// 激励视频广告收益事件名称，默认值为"ad_purchase"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string RewardedVideoRevenuePaid { get; set; } = "ad_purchase";
        /// <summary>
        /// 激励视频广告完成事件名称，默认值为"ad_rewarded_completed"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string RewardedVideoCompleted { get; set; } = "ad_rewarded_completed";

        #endregion

        #region InterstitialVideo
        /// <summary>
        /// 插屏广告加载完成事件名称，默认值为"ad_interstitial_loaded"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string InterstitialVideoLoaded { get; set; } = "ad_interstitial_loaded";
        /// <summary>
        /// 插屏广告加载失败事件名称，默认值为"ad_interstitial_load_failed"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string InterstitialVideoLoadFailed { get; set; } = "ad_interstitial_load_failed";
        /// <summary>
        /// 插屏广告展示完成事件名称，默认值为"ad_interstitial_shown"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string InterstitialVideoShown { get; set; } = "ad_interstitial_shown";
        /// <summary>
        /// 插屏广告展示失败事件名称，默认值为"ad_interstitial_show_failed"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string InterstitialVideoShowFailed { get; set; } = "ad_interstitial_show_failed";
        /// <summary>
        /// 插屏广告收益事件名称，默认值为"ad_purchase"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string InterstitialVideoRevenuePaid { get; set; } = "ad_purchase";
        /// <summary>
        /// 插屏广告关闭事件名称，默认值为"ad_interstitial_closed"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string InterstitialVideoDismissed { get; set; } = "ad_interstitial_closed";
        /// <summary>
        /// 插屏广告点击事件名称，默认值为"ad_interstitial_clicked"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string InterstitialVideoClicked { get; set; } = "ad_interstitial_clicked";
        /// <summary>
        /// 插屏广告完成事件名称，默认值为"ad_interstitial_completed"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string InterstitialVideoCompleted { get; set; } = "ad_interstitial_completed";
        #endregion

        #region Banner
        /// <summary>
        /// Banner广告加载完成事件名称，默认值为"ad_banner_loaded"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string BannerLoaded { get; set; } = "ad_banner_loaded";
        /// <summary>
        /// Banner广告加载失败事件名称，默认值为"ad_banner_load_failed"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string BannerLoadFailed { get; set; } = "ad_banner_load_failed";
        /// <summary>
        /// Banner广告点击事件名称，默认值为"ad_banner_clicked"
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string BannerClicked { get; set; } = "ad_banner_clicked";
        /// <summary>
        /// Banner广告收益事件名称，默认值为""
        /// </summary>
        /// <remarks>
        /// 如果为空，则表示不记录此事件
        /// </remarks>
        public static string BannerRevenuePaid { get; set; } = "ad_purchase";
        #endregion
    }
}
