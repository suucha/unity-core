using SuuchaStudio.Unity.Core.Ioc;

namespace SuuchaStudio.Unity.Core.AdPlaying
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Suucha.Unity.Core.SuuchaBase" />
    /// <seealso cref="Suucha.Unity.Core.AdPlaying.IAdPlayingStrategy" />
    public abstract class AdPlayingStrategyAbstract : SuuchaBase, IAdPlayingStrategy
    {
        private IRewardedVideoPlayer rewardedVideoPlayer;
        /// <summary>
        /// Gets the rewarded video player.
        /// </summary>
        /// <value>
        /// The rewarded video player.
        /// </value>
        protected IRewardedVideoPlayer RewardedVideoPlayer
        {
            get
            {
                if (rewardedVideoPlayer != null)
                {
                    return rewardedVideoPlayer;
                }
                if (!IocContainer.TryResolve<IRewardedVideoPlayer>
                    (out rewardedVideoPlayer))
                {
                    return EmptyRewardedVideoPlayer.Instance;
                }
                rewardedVideoPlayer.OnLoadFailed += RewardedVideoPlayerOnFailed;
                rewardedVideoPlayer.OnClosed += RewardedVideoPlayerOnShown;
                rewardedVideoPlayer.OnLoaded += RewardedVideoPlayerOnLoaded;
                rewardedVideoPlayer.OnShowFailed += RewardedVideoPlayerOnShowFailed;
                return rewardedVideoPlayer;
            }
        }
        private IInterstitialVideoPlayer interstitialVideoPlayer;
        /// <summary>
        /// Gets the interstitial video player.
        /// </summary>
        /// <value>
        /// The interstitial video player.
        /// </value>
        protected IInterstitialVideoPlayer InterstitialVideoPlayer
        {
            get
            {
                if (interstitialVideoPlayer != null)
                {
                    return interstitialVideoPlayer;
                }
                if (!IocContainer.TryResolve<IInterstitialVideoPlayer>(out interstitialVideoPlayer))
                {
                    return EmptyInterstitialVideoPlayer.Instance;
                }
                interstitialVideoPlayer.OnLoadFailed += InterstitialVideoPlayerOnLoadFailed;
                interstitialVideoPlayer.OnDismissed += InterstitialVideoPlayerOnShown;
                interstitialVideoPlayer.OnLoaded += InterstitialVideoPlayerOnLoaded;
                interstitialVideoPlayer.OnShowFailed += InterstitialVideoPlayerOnShowFailed;
                return interstitialVideoPlayer;
            }
        }
        private IBannerPlayer bannerPlayer;
        /// <summary>
        /// Gets the banner player.
        /// </summary>
        /// <value>
        /// The banner player.
        /// </value>
        protected IBannerPlayer BannerPlayer
        {
            get
            {
                if (bannerPlayer != null)
                {
                    return bannerPlayer;
                }
                if (!IocContainer.TryResolve<IBannerPlayer>(out bannerPlayer))
                {
                    return EmptyBannerPlayer.Instance;
                }
                return bannerPlayer;
            }
        }

        /// <summary>
        /// 策略名称
        /// </summary>
        public abstract string StrategyName { get; }

        /// <summary>
        /// 根据广告位置获取Banner广告单元ID
        /// </summary>
        /// <param name="placement">广告位置</param>
        /// <param name="adGetResults">获取结果</param>
        /// <returns>
        /// 如果有对应的广告单元，则返回广告单元ID，否则返回空，同时adGetResults返回原因
        /// </returns>
        public abstract string GetBannerAdUnitId(string placement, out AdRequestResults adGetResults);

        /// <summary>
        /// 根据广告位置获取插屏广告单元ID
        /// </summary>
        /// <param name="placement">广告位置</param>
        /// <param name="adGetResults">获取结果</param>
        /// <returns>
        /// 如果有对应的广告单元，则返回广告单元ID，否则返回空，同时adGetResults返回原因
        /// </returns>
        public abstract string GetInterstitialVideoAdUnitId(string placement, out AdRequestResults adGetResults);

        /// <summary>
        /// 根据广告位置获取激励视频广告单元ID
        /// </summary>
        /// <param name="placement">广告位置</param>
        /// <param name="adGetResults">获取结果</param>
        /// <returns>
        /// 如果有对应的广告单元，则返回广告单元ID，否则返回空，同时adGetResults返回原因
        /// </returns>
        public abstract string GetRewardedVideoAdUnitId(string placement, out AdRequestResults adGetResults);
        /// <summary>
        /// The rewardeds video player on failed.
        /// </summary>
        /// <param name="adUnitId">The ad unit identifier.</param>
        /// <param name="errorMessage">The error message.</param>
        protected abstract void RewardedVideoPlayerOnFailed(string adUnitId, string errorMessage);
        /// <summary>
        /// The rewardeds video player on shown.
        /// </summary>
        /// <param name="adUnitId">The ad unit identifier.</param>
        /// <param name="adCallbackInfo">The ad callback information.</param>
        protected abstract void RewardedVideoPlayerOnShown(string adUnitId, AdCallbackInfo adCallbackInfo);
        /// <summary>
        /// The rewardeds video player on show failed.
        /// </summary>
        /// <param name="adUnitId">The ad unit identifier.</param>
        /// <param name="error">The error.</param>
        /// <param name="placement">The placement.</param>
        protected abstract void RewardedVideoPlayerOnShowFailed(string adUnitId, string error, string placement);
        /// <summary>
        /// The rewardeds video player on loaded.
        /// </summary>
        /// <param name="adUnitId">The ad unit identifier.</param>
        /// <param name="adCallbackInfo">The ad callback information.</param>
        protected abstract void RewardedVideoPlayerOnLoaded(string adUnitId, AdCallbackInfo adCallbackInfo);
        /// <summary>
        /// Interstitials the video player on show failed.
        /// </summary>
        /// <param name="adUnitId">The ad unit identifier.</param>
        /// <param name="error">The error.</param>
        /// <param name="placement">The placement.</param>
        protected abstract void InterstitialVideoPlayerOnShowFailed(string adUnitId, string error, string placement);
        /// <summary>
        /// The interstitials video player on loaded.
        /// </summary>
        /// <param name="adUnitId">The ad unit identifier.</param>
        /// <param name="adCallbackInfo">The ad callback information.</param>
        protected abstract void InterstitialVideoPlayerOnLoaded(string adUnitId, AdCallbackInfo adCallbackInfo);
        /// <summary>
        /// The interstitials video player on shown.
        /// </summary>
        /// <param name="adUnitId">The ad unit identifier.</param>
        /// <param name="adCallbackInfo">The ad callback information.</param>
        protected abstract void InterstitialVideoPlayerOnShown(string adUnitId, AdCallbackInfo adCallbackInfo);
        /// <summary>
        /// The interstitials video player on load failed.
        /// </summary>
        /// <param name="adUnitId">The ad unit identifier.</param>
        /// <param name="errorMessage">The error message.</param>
        protected abstract void InterstitialVideoPlayerOnLoadFailed(string adUnitId, string errorMessage);

    }
}
