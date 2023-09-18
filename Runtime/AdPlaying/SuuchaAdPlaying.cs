using SuuchaStudio.Unity.Core.Ioc;
using SuuchaStudio.Unity.Core.LogEvents;
using SuuchaStudio.Unity.Core.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Threading.Tasks;

namespace SuuchaStudio.Unity.Core.AdPlaying
{
    public class SuuchaAdPlaying : SuuchaBase
    {
        bool isInitialled = false;
        bool enableLogEvent = false;
        List<AdPlayingEnableEvents> enablePlacementEvents;
        List<AdPlayingEnableEvents> enableEvents;
        private readonly IAdPlayerManager adPlayerManager;
        IAdPlayingStrategy adPlayingStrategy;

        IRewardedVideoPlayer rewardedVideoPlayer;
        IRewardedVideoPlayer RewardedVideoPlayer
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
                rewardedVideoPlayer.OnClicked += RewardedVideoPlayerOnClickedInternal;
                rewardedVideoPlayer.OnLoadFailed += RewardedVideoPlayerOnLoadFailedInternal;
                rewardedVideoPlayer.OnShowFailed += RewardedVideoPlayerOnShowFailedInternal;
                rewardedVideoPlayer.OnLoaded += RewardedVideoPlayerOnLoadedInternal;
                rewardedVideoPlayer.OnRevenuePaid += RewardedVideoPlayerOnRevenuePaidInternal;
                rewardedVideoPlayer.OnClosed += RewardedVideoPlayerOnClosedInternal;
                rewardedVideoPlayer.OnShown += RewardedVideoPlayerOnShownInternal;
                rewardedVideoPlayer.OnCompleted += RewardedVideoPlayerOnCompletedInternal;
                return rewardedVideoPlayer;
            }
        }



        IInterstitialVideoPlayer interstitialVideoPlayer;
        IInterstitialVideoPlayer InterstitialVideoPlayer
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
                interstitialVideoPlayer.OnClicked += InterstitialVideoPlayerOnClickedInternal;
                interstitialVideoPlayer.OnLoadFailed += InterstitialVideoPlayerOnLoadFailedInternal;
                interstitialVideoPlayer.OnShowFailed += InterstitialVideoPlayerOnFailedToPlayInternal;
                interstitialVideoPlayer.OnLoaded += InterstitialVideoPlayerOnLoadedInternal;
                interstitialVideoPlayer.OnRevenuePaid += InterstitialVideoPlayerOnRevenuePaidInternal;
                interstitialVideoPlayer.OnDismissed += InterstitialVideoPlayerOnDismissedInternal;
                interstitialVideoPlayer.OnShown += InterstitialVideoPlayerOnShownInternal;
                interstitialVideoPlayer.OnCompleted += InterstitialVideoPlayerOnCompletedInternal;
                return interstitialVideoPlayer;
            }
        }



        IBannerPlayer bannerPlayer;
        IBannerPlayer BannerPlayer
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
                bannerPlayer.OnClicked += BannerPlayerOnClickedInternal;
                bannerPlayer.OnLoadFailed += BannerPlayerOnLoadFailedInternal;
                bannerPlayer.OnLoaded += BannerPlayerOnLoadedInternal;
                bannerPlayer.OnRevenuePaid += BannerPlayerOnRevenuePaidInternal;
                return bannerPlayer;
            }
        }
        public SuuchaAdPlaying(IAdPlayerManager adPlayerManager, IAdPlayingStrategy adPlayingStrategy)
        {
            this.adPlayerManager = adPlayerManager ?? throw new ArgumentNullException("adPlayerManager");
            this.adPlayingStrategy = adPlayingStrategy ?? throw new ArgumentNullException("adPlayingStrategy");
            InitializeAdPlayerManager();

        }
        #region Banner events
        async void BannerPlayerOnRevenuePaidInternal(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            if (enableLogEvent &&  enableEvents.Contains(AdPlayingEnableEvents.BannerRevenuePaid))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.BannerRevenuePaid}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId },
                         { EventParameterNames.AdPlacement, adCallbackInfo.Placement},
                         { EventParameterNames.AdLabel, adCallbackInfo.NetworkPlacement},
                         { EventParameterNames.AdType, "banner" },
                         { EventParameterNames.PurchaseType,"ad" },
                         { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) }
                     });
                if (!string.IsNullOrEmpty(adCallbackInfo.Placement)
                    && enablePlacementEvents.Contains(AdPlayingEnableEvents.BannerRevenuePaid))
                {
                    await LogEventManager.LogEvent($"{AdPlayingEventNames.BannerRevenuePaid}_{adCallbackInfo.Placement}",
                         new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId },
                         { EventParameterNames.AdPlacement, adCallbackInfo.Placement},
                         { EventParameterNames.AdLabel, adCallbackInfo.NetworkPlacement},
                         { EventParameterNames.AdType, "banner" },
                         { EventParameterNames.PurchaseType,"ad" },
                         { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) }
                         });
                }
            }
            try
            {
                SuuchaAdPlayingCallbacks.BannerRevenuePaidHandler?.Invoke(adUnitId, adCallbackInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On banner revenue paid callback error:{ex.Message}");
            }
        }

        async void BannerPlayerOnLoadedInternal(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.BannerLoaded))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.BannerLoaded}",
                    new Dictionary<string, string> {
                        { EventParameterNames.AdUnitId, adUnitId },
                        { EventParameterNames.AdType, "banner" },
                        { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                        { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) } });
                if (!string.IsNullOrEmpty(adCallbackInfo.Placement)
                    && enablePlacementEvents.Contains(AdPlayingEnableEvents.BannerLoaded))
                {
                    await LogEventManager.LogEvent($"{AdPlayingEventNames.BannerLoaded}_{adCallbackInfo.Placement}",
                         new Dictionary<string, string> {
                             { EventParameterNames.AdUnitId, adUnitId },
                             { EventParameterNames.AdType, "banner" },
                             { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                             { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) } });
                }
            }
            try
            {
                SuuchaAdPlayingCallbacks.BannerLoadedHandler?.Invoke(adUnitId, adCallbackInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On banner loaded callback error:{ex.Message}");
            }
        }

        async void BannerPlayerOnLoadFailedInternal(string adUnitId, string errorMessage)
        {
            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.BannerLoaded))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.BannerLoadFailed}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId },
                         { EventParameterNames.AdType, "banner" } });
            }
            try
            {
                SuuchaAdPlayingCallbacks.BannerFailedHandler?.Invoke(adUnitId, errorMessage);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On banner load failed callback error:{ex.Message}");
            }
        }

        async void BannerPlayerOnClickedInternal(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.BannerClicked))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.BannerClicked}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId } ,
                         { EventParameterNames.AdType, "banner" } ,
                         { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                         { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) } });
                if (!string.IsNullOrEmpty(adCallbackInfo.Placement)
                    && enablePlacementEvents.Contains(AdPlayingEnableEvents.BannerClicked))
                {
                    await LogEventManager.LogEvent($"{AdPlayingEventNames.BannerClicked}_{adCallbackInfo.Placement}",
                        new Dictionary<string, string> {
                            { EventParameterNames.AdUnitId, adUnitId } ,
                            { EventParameterNames.AdType, "banner" } ,
                            { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                            { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) } });
                }
            }
            try
            {
                SuuchaAdPlayingCallbacks.BannerClickedHandler?.Invoke(adUnitId, adCallbackInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On banner clicked callback error:{ex.Message}");
            }
        }
        #endregion
        #region Interstitial video events
        async void InterstitialVideoPlayerOnRevenuePaidInternal(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.InterstitialVideoRevenuePaid))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.InterstitialVideoRevenuePaid}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId },
                         { EventParameterNames.AdPlacement, adCallbackInfo.Placement},
                         { EventParameterNames.AdLabel, adCallbackInfo.NetworkPlacement},
                         { EventParameterNames.AdType, "interstitial" },
                         { EventParameterNames.PurchaseType,"ad" },
                         { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) }
                     });
                if (!string.IsNullOrEmpty(adCallbackInfo.Placement)
                    && enablePlacementEvents.Contains(AdPlayingEnableEvents.InterstitialVideoRevenuePaid))
                {
                    await LogEventManager.LogEvent($"{AdPlayingEventNames.InterstitialVideoRevenuePaid}_{adCallbackInfo.Placement}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId },
                         { EventParameterNames.AdPlacement, adCallbackInfo.Placement},
                         { EventParameterNames.AdLabel, adCallbackInfo.NetworkPlacement},
                         { EventParameterNames.AdType, "interstitial" },
                         { EventParameterNames.PurchaseType,"ad" },
                         { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) }
                     });
                }
            }
            try
            {
                SuuchaAdPlayingCallbacks.InterstitialVideoRevenuePaidHandler?.Invoke(adUnitId, adCallbackInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On interstitial video revenue paid callback error:{ex.Message}");
            }
        }

        async void InterstitialVideoPlayerOnLoadedInternal(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.InterstitialVideoLoaded))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.InterstitialVideoLoaded}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId },
                         { EventParameterNames.AdType, "interstitial" },
                         { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                         { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) } });
                if (!string.IsNullOrEmpty(adCallbackInfo.Placement) &&
                    enablePlacementEvents.Contains(AdPlayingEnableEvents.InterstitialVideoLoaded))
                {
                    await LogEventManager.LogEvent($"{AdPlayingEventNames.InterstitialVideoLoaded}_{adCallbackInfo.Placement}",
                         new Dictionary<string, string> {
                             { EventParameterNames.AdUnitId, adUnitId },
                             { EventParameterNames.AdType, "interstitial" },
                             { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                             { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) } });
                }
            }

            try
            {
                SuuchaAdPlayingCallbacks.InterstitialVideoLoadedHandler?.Invoke(adUnitId, adCallbackInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On interstitial video loaded callback error:{ex.Message}");
            }
        }

        async void InterstitialVideoPlayerOnFailedToPlayInternal(string adUnitId, string errorMessage, string placement)
        {
            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.InterstitialVideoShowFailed))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.InterstitialVideoShowFailed}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId },
                         { EventParameterNames.AdType, "interstitial" },
                         { EventParameterNames.AdPlacement, placement } });
                if (!string.IsNullOrEmpty(placement) &&
                    enablePlacementEvents.Contains(AdPlayingEnableEvents.InterstitialVideoShowFailed))
                {
                    await LogEventManager.LogEvent($"{AdPlayingEventNames.InterstitialVideoShowFailed}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId },
                         { EventParameterNames.AdType, "interstitial" },
                         { EventParameterNames.AdPlacement, placement } });
                }
            }
            try
            {
                SuuchaAdPlayingCallbacks.InterstitialVideoShowFailedHandler?.Invoke(adUnitId, errorMessage, placement);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On interstitial video failed to play callback error:{ex.Message}");
            }
        }

        async void InterstitialVideoPlayerOnLoadFailedInternal(string adUnitId, string errorMessage)
        {
            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.InterstitialVideoLoadFailed))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.InterstitialVideoLoadFailed}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId },
                         { EventParameterNames.AdType, "interstitial" } });
            }
            try
            {
                SuuchaAdPlayingCallbacks.InterstitialVideoLoadFailedHandler?.Invoke(adUnitId, errorMessage);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On interstitial video load failed callback error:{ex.Message}");
            }
        }

        async void InterstitialVideoPlayerOnClickedInternal(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.InterstitialVideoClicked))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.InterstitialVideoClicked}",
                 new Dictionary<string, string> {
                     { EventParameterNames.AdUnitId, adUnitId },
                     { EventParameterNames.AdType, "interstitial" },
                     { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                     { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) } });
                if (!string.IsNullOrEmpty(adCallbackInfo.Placement) &&
                    enablePlacementEvents.Contains(AdPlayingEnableEvents.InterstitialVideoClicked))
                {
                    await LogEventManager.LogEvent($"{AdPlayingEventNames.InterstitialVideoClicked}_{adCallbackInfo.Placement}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId },
                         { EventParameterNames.AdType, "interstitial" },
                         { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                         { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) } });
                }
            }
            try
            {
                SuuchaAdPlayingCallbacks.InterstitialVideoClickedHandler?.Invoke(adUnitId, adCallbackInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On interstitial video clicked callback error:{ex.Message}");
            }
        }
        async void InterstitialVideoPlayerOnDismissedInternal(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.InterstitialVideoDismissed))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.InterstitialVideoDismissed}",
                    new Dictionary<string, string> {
                        { EventParameterNames.AdUnitId, adUnitId },
                        { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                        { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) }  });
                if (!string.IsNullOrEmpty(adCallbackInfo.Placement) &&
                    enablePlacementEvents.Contains(AdPlayingEnableEvents.InterstitialVideoDismissed))
                {
                    await LogEventManager.LogEvent($"{AdPlayingEventNames.InterstitialVideoDismissed}_{adCallbackInfo.Placement}",
                        new Dictionary<string, string> {
                            { EventParameterNames.AdUnitId, adUnitId },
                            { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                            { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) }  });
                }
            }
            try
            {
                SuuchaAdPlayingCallbacks.InterstitialVideoDismissedHandler?.Invoke(adUnitId, adCallbackInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On interstitial video dismissed callback error:{ex.Message}");
            }
        }
        async void InterstitialVideoPlayerOnShownInternal(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.InterstitialVideoShown))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.InterstitialVideoShown}",
                    new Dictionary<string, string> {
                        { EventParameterNames.AdUnitId, adUnitId } ,
                        { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                        { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) }  });
                if (!string.IsNullOrEmpty(adCallbackInfo.Placement) && enablePlacementEvents.Contains(AdPlayingEnableEvents.InterstitialVideoShown))
                {
                    await LogEventManager.LogEvent($"{AdPlayingEventNames.InterstitialVideoShown}_{adCallbackInfo.Placement}",
                    new Dictionary<string, string> {
                        { EventParameterNames.AdUnitId, adUnitId } ,
                        { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                        { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) }  });
                }
            }
            try
            {
                SuuchaAdPlayingCallbacks.InterstitialVideoShownHandler?.Invoke(adUnitId, adCallbackInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On interstitial video shown callback error:{ex.Message}");
            }
        }
        async void InterstitialVideoPlayerOnCompletedInternal(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.InterstitialVideoCompleted))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.InterstitialVideoCompleted}",
                    new Dictionary<string, string> {
                        { EventParameterNames.AdUnitId, adUnitId } ,
                        { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                        { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) } });
                if (!string.IsNullOrEmpty(adCallbackInfo.Placement) &&
                    enablePlacementEvents.Contains(AdPlayingEnableEvents.InterstitialVideoCompleted))
                {
                    await LogEventManager.LogEvent($"{AdPlayingEventNames.InterstitialVideoCompleted}_{adCallbackInfo.Placement}",
                        new Dictionary<string, string> {
                            { EventParameterNames.AdUnitId, adUnitId } ,
                            { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                            { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) } });
                }
            }
            try
            {
                SuuchaAdPlayingCallbacks.InterstitialVideoCompletedHandler?.Invoke(adUnitId, adCallbackInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On interstitial video completed callback error:{ex.Message}");
            }
        }
        #endregion
        #region Rewarded video events
        async void RewardedVideoPlayerOnShownInternal(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.RewardedVideoShown))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.RewardedVideoShown}",
                    new Dictionary<string, string> {
                        { EventParameterNames.AdUnitId, adUnitId },
                        { EventParameterNames.AdType, "rewarded" },
                        { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                        { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) } });
                if (!string.IsNullOrEmpty(adCallbackInfo.Placement) &&
                    enablePlacementEvents.Contains(AdPlayingEnableEvents.RewardedVideoShown))
                {
                    await LogEventManager.LogEvent($"{AdPlayingEventNames.RewardedVideoShown}_{adCallbackInfo.Placement}",
                         new Dictionary<string, string> {
                             { EventParameterNames.AdUnitId, adUnitId },
                             { EventParameterNames.AdType, "rewarded" },
                             { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                             { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) } });
                }
            }
            try
            {
                SuuchaAdPlayingCallbacks.RewardedVideoShownHandler?.Invoke(adUnitId, adCallbackInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On rewarded video shown callback error:{ex.Message}");
            }
            RequestRewardedVideo(adCallbackInfo.Placement);
        }
        async void RewardedVideoPlayerOnClosedInternal(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.RewardedVideoClosed))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.RewardedVideoClosed}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId } ,
                         { EventParameterNames.AdType, "rewarded" } ,
                         { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                         { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) } });
                if (!string.IsNullOrEmpty(adCallbackInfo.Placement) &&
                    enablePlacementEvents.Contains(AdPlayingEnableEvents.RewardedVideoClosed))
                {
                    await LogEventManager.LogEvent($"{AdPlayingEventNames.RewardedVideoClosed}_{adCallbackInfo.Placement}",
                         new Dictionary<string, string> {
                             { EventParameterNames.AdUnitId, adUnitId } ,
                             { EventParameterNames.AdType, "rewarded" } ,
                             { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                             { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) } });
                }
            }
            try
            {
                SuuchaAdPlayingCallbacks.RewardedVideoClosedHandler?.Invoke(adUnitId, adCallbackInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On rewarded video closed callback error:{ex.Message}");
            }
            RequestRewardedVideo(adCallbackInfo.Placement);
        }
        async void RewardedVideoPlayerOnRevenuePaidInternal(string adUnitId, AdCallbackInfo adCallbackInfo)
        {

            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.RewardedVideoRevenuePaid))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.RewardedVideoRevenuePaid}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId },
                         { EventParameterNames.AdPlacement, adCallbackInfo.Placement},
                         { EventParameterNames.AdLabel, adCallbackInfo.NetworkPlacement},
                         { EventParameterNames.AdType, "rewarded" },
                         { EventParameterNames.PurchaseType,"ad" },
                         { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) }
                     });
                if (!string.IsNullOrEmpty(adCallbackInfo.Placement) &&
                    enablePlacementEvents.Contains(AdPlayingEnableEvents.RewardedVideoRevenuePaid))
                {
                    await LogEventManager.LogEvent($"{AdPlayingEventNames.RewardedVideoRevenuePaid}_{adCallbackInfo.Placement}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId },
                         { EventParameterNames.AdPlacement, adCallbackInfo.Placement},
                         { EventParameterNames.AdLabel, adCallbackInfo.NetworkPlacement},
                         { EventParameterNames.AdType, "rewarded" },
                         { EventParameterNames.PurchaseType,"ad" },
                         { AdPlayingEventNames.RevenuePaidEventParameterName,adCallbackInfo.Revenue.ToString("F10",CultureInfo.InvariantCulture) }
                     });
                }
            }
            try
            {
                SuuchaAdPlayingCallbacks.RewardedVideoRevenuePaidHandler?.Invoke(adUnitId, adCallbackInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On rewarded video revenue paid callback error:{ex.Message}");
            }
            RequestRewardedVideo(adCallbackInfo.Placement);
        }

        async void RewardedVideoPlayerOnLoadedInternal(string adUnitId, AdCallbackInfo adCallbackInfo)
        {

            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.RewardedVideoLoaded))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.RewardedVideoLoaded}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId },
                         { EventParameterNames.AdType, "rewarded" },
                         { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                         { AdPlayingEventNames.RevenuePaidEventParameterName, adCallbackInfo.Revenue.ToString("F10", CultureInfo.InvariantCulture) } });
                if (!string.IsNullOrEmpty(adCallbackInfo.Placement) &&
                    enablePlacementEvents.Contains(AdPlayingEnableEvents.RewardedVideoLoaded))
                {
                    await LogEventManager.LogEvent($"{AdPlayingEventNames.RewardedVideoLoaded}_{adCallbackInfo.Placement}",
                         new Dictionary<string, string> {
                             { EventParameterNames.AdUnitId, adUnitId },
                             { EventParameterNames.AdType, "rewarded" },
                             { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                             { AdPlayingEventNames.RevenuePaidEventParameterName, adCallbackInfo.Revenue.ToString("F10", CultureInfo.InvariantCulture) } });
                }
            }
            try
            {
                SuuchaAdPlayingCallbacks.RewardedVideoLoadedHandler?.Invoke(adUnitId, adCallbackInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On rewarded video loaded callback error:{ex.Message}");
            }
        }

        async void RewardedVideoPlayerOnShowFailedInternal(string adUnitId, string error, string placement)
        {

            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.RewardedVideoShowFailed))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.RewardedVideoShowFailed}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId },
                         { EventParameterNames.AdType, "rewarded" },
                         { EventParameterNames.AdPlacement, placement } });
                if (!string.IsNullOrEmpty(placement) &&
                    enablePlacementEvents.Contains(AdPlayingEnableEvents.RewardedVideoShowFailed))
                {
                    await LogEventManager.LogEvent($"{AdPlayingEventNames.RewardedVideoShowFailed}_{placement}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId },
                         { EventParameterNames.AdType, "rewarded" },
                         { EventParameterNames.AdPlacement, placement } });
                }
            }
            try
            {
                SuuchaAdPlayingCallbacks.RewardedVideoShowFailedHandler?.Invoke(adUnitId, error, placement);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On rewarded video failed to play callback error:{ex.Message}");
            }
            RequestRewardedVideo(placement);
        }

        async void RewardedVideoPlayerOnLoadFailedInternal(string adUnitId, string errorMessage)
        {

            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.RewardedVideoLoadFailed))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.RewardedVideoLoadFailed}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdType, "rewarded" },
                         { EventParameterNames.AdUnitId, adUnitId } });
            }
            try
            {
                SuuchaAdPlayingCallbacks.RewardedVideoFailedHandler?.Invoke(adUnitId, errorMessage);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On rewarded video load failed callback error:{ex.Message}");
            }
        }

        async void RewardedVideoPlayerOnClickedInternal(string adUnitId, AdCallbackInfo adCallbackInfo)
        {

            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.RewardedVideoClicked))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.RewardedVideoClicked}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId },
                         { EventParameterNames.AdType, "rewarded" },
                         { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                         { AdPlayingEventNames.RevenuePaidEventParameterName, adCallbackInfo.Revenue.ToString("F10", CultureInfo.InvariantCulture) } });
                if (!string.IsNullOrEmpty(adCallbackInfo.Placement) &&
                    enablePlacementEvents.Contains(AdPlayingEnableEvents.RewardedVideoClicked))
                {
                    await LogEventManager.LogEvent($"{AdPlayingEventNames.RewardedVideoClicked}_{adCallbackInfo.Placement}",
                         new Dictionary<string, string> {
                             { EventParameterNames.AdUnitId, adUnitId },
                             { EventParameterNames.AdType, "rewarded" },
                             { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                             { AdPlayingEventNames.RevenuePaidEventParameterName, adCallbackInfo.Revenue.ToString("F10", CultureInfo.InvariantCulture) } });
                }
            }
            try
            {
                SuuchaAdPlayingCallbacks.RewardedVideoClickedHandler?.Invoke(adUnitId, adCallbackInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On rewarded video clicked callback error:{ex.Message}");
            }
        }
        async void RewardedVideoPlayerOnCompletedInternal(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            if (enableLogEvent && enableEvents.Contains(AdPlayingEnableEvents.RewardedVideoCompleted))
            {
                await LogEventManager.LogEvent($"{AdPlayingEventNames.RewardedVideoCompleted}",
                     new Dictionary<string, string> {
                         { EventParameterNames.AdUnitId, adUnitId } ,
                         { EventParameterNames.AdType, "rewarded" } ,
                         { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                         { AdPlayingEventNames.RevenuePaidEventParameterName, adCallbackInfo.Revenue.ToString("F10", CultureInfo.InvariantCulture) }  });
                if (!string.IsNullOrEmpty(adCallbackInfo.Placement) &&
                     enablePlacementEvents.Contains(AdPlayingEnableEvents.RewardedVideoCompleted))
                {
                    await LogEventManager.LogEvent($"{AdPlayingEventNames.RewardedVideoCompleted}_{adCallbackInfo.Placement}",
                         new Dictionary<string, string> {
                             { EventParameterNames.AdUnitId, adUnitId } ,
                             { EventParameterNames.AdType, "rewarded" } ,
                             { EventParameterNames.AdPlacement, adCallbackInfo.Placement },
                             { AdPlayingEventNames.RevenuePaidEventParameterName, adCallbackInfo.Revenue.ToString("F10", CultureInfo.InvariantCulture) }  });
                }
            }
            try
            {
                SuuchaAdPlayingCallbacks.RewardedVideoCompletedHandler?.Invoke(adUnitId, adCallbackInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError($"On rewarded video completed callback error:{ex.Message}");
            }
            RequestRewardedVideo(adCallbackInfo.Placement);
        }
        #endregion
        private bool InitializeAdPlayerManager()
        {
            //this.adPlayingStrategy = adPlayingStrategy;
            if (isInitialled)
            {
                Logger.LogWarning($"Advertising SDK has been initialized.");
                return isInitialled;
            }
            isInitialled = adPlayerManager.Initialize().AsTask().Result;
            if (isInitialled)
            {
                RequestAds();
            }
            return isInitialled;
        }
        public void EnableAdPlayingEvent(List<AdPlayingEnableEvents> enableEvents, List<AdPlayingEnableEvents> enablePlacementEvents)
        {
            enableLogEvent = true;
            this.enableEvents = enableEvents ?? new List<AdPlayingEnableEvents>();
            this.enablePlacementEvents = enablePlacementEvents ?? new List<AdPlayingEnableEvents>();
        }
        void RequestAds()
        {
            RequestRewardedVideo();
            RequestInterstitialVideo();

            var adUnitId = adPlayingStrategy.GetBannerAdUnitId(null, out var adRequestResults);
            if (!string.IsNullOrEmpty(adUnitId))
            {
                BannerPlayer.RequestBanner(adUnitId, BannerPosition.BottomCenter);
            }
        }
        /// <summary>
        /// Checks if a rewarded video ad is available for the specified placement.
        /// </summary>
        /// <param name="placement">The name of the ad placement.</param>
        /// <returns>The result of the ad request, indicating whether a rewarded video ad is available.</returns>
        public AdRequestResults HasRewardedVideo(string placement)
        {
            Logger.LogDebug($"HasRewardedVideo started for placement: {placement}");
            if (!isInitialled)
            {
                Logger.LogError($"HasRewardedVideo: The advertising SDK is not initialized.");
                return AdRequestResults.SdkNotInitialized;
            }
            var adUnitId = adPlayingStrategy.GetRewardedVideoAdUnitId(placement, out AdRequestResults adRequestResults);
            if (string.IsNullOrEmpty(adUnitId))
            {
                Logger.LogWarning($"No ad unit ID found for rewarded video, placement: {placement}");
                return adRequestResults;
            }
            var hasAd = RewardedVideoPlayer.HasRewardedVideo(adUnitId);
            if (hasAd)
            {
                Logger.LogDebug($"Rewarded video ad available for ad unit ID: {adUnitId}, placement: {placement}");
                return AdRequestResults.Successful;
            }
            Logger.LogDebug($"Rewarded video ad not ready for ad unit ID: {adUnitId}, placement: {placement}");
            return AdRequestResults.NotReady;
        }
        /// <summary>
        /// Attempts to show a rewarded video ad for the specified placement.
        /// </summary>
        /// <param name="placement">The name of the ad placement.</param>
        /// <returns>The result of the ad request, indicating whether the rewarded video ad was shown or not.</returns>
        public AdRequestResults ShowRewardedVideo(string placement)
        {
            Logger.LogDebug($"ShowRewardedVideo started for placement: {placement}");
            var adRequestResults = AdRequestResults.Successful;
            if (!isInitialled)
            {
                adRequestResults = AdRequestResults.SdkNotInitialized;
                Logger.LogError($"ShowRewardedVideo: The advertising SDK is not initialized.");
                return adRequestResults;
            }
            var adUnitId = adPlayingStrategy.GetRewardedVideoAdUnitId(placement, out adRequestResults);
            if (string.IsNullOrEmpty(adUnitId))
            {
                Logger.LogWarning($"Has no ad unit ID for rewarded, placement:{placement}");
                return adRequestResults;
            }
            Logger.LogDebug($"Ad unit ID for rewarded, placement: {placement}, ad unit ID: {adUnitId}");
            if (RewardedVideoPlayer.HasRewardedVideo(adUnitId))
            {
                Logger.LogDebug($"Show rewarded video for ad unit ID: {adUnitId}, placement: {placement}");
                RewardedVideoPlayer.ShowRewardedVideo(adUnitId, placement);
                return adRequestResults;
            }
            adRequestResults = AdRequestResults.NotReady;
            Logger.LogWarning($"Rewarded video ad not ready, requesting ad for ad unit ID: {adUnitId}");
            RewardedVideoPlayer.RequestRewardedVideo(adUnitId);
            return adRequestResults;
        }
        public void RequestRewardedVideo(string placement = "")
        {
            Logger.LogDebug($"RequestRewardedVideo started for placement: {placement}");
            var adUnitId = adPlayingStrategy.GetRewardedVideoAdUnitId(placement, out AdRequestResults adRequestResults);
            if (string.IsNullOrEmpty(adUnitId))
            {
                Logger.LogWarning($"Has no ad unit ID for rewarded, placement:{placement}, result: {adRequestResults}");
                return;
            }
            if (!RewardedVideoPlayer.HasRewardedVideo(adUnitId))
            {
                Logger.LogDebug($"Requesting rewarded video ad for ad unit ID: {adUnitId}");
                RewardedVideoPlayer.RequestRewardedVideo(adUnitId);
            }
        }
        /// <summary>
        /// Checks if an interstitial video ad is available for the specified placement.
        /// </summary>
        /// <param name="placement">The name of the ad placement.</param>
        /// <returns>The result of the ad request, indicating whether an ad is available.</returns>
        public AdRequestResults HasInterstitialVideo(string placement)
        {
            Logger.LogDebug($"HasInterstitialVideo started for placement: {placement}");
            if (!isInitialled)
            {
                Logger.LogError($"HasInterstitialVideo: The advertising SDK is not initialized.");
                return AdRequestResults.SdkNotInitialized;
            }
            var adUnitId = adPlayingStrategy.GetInterstitialVideoAdUnitId(placement, out AdRequestResults adRequestResults);
            if (string.IsNullOrEmpty(adUnitId))
            {
                Logger.LogWarning($"No ad unit ID found for interstitial video, placement: {placement}");
                return adRequestResults;
            }
            var hasAd = InterstitialVideoPlayer.HasInterstitialVideo(adUnitId);
            if (hasAd)
            {
                Logger.LogDebug($"Interstitial video ad available for ad unit ID: {adUnitId}, placement: {placement}");
                return AdRequestResults.Successful;
            }
            Logger.LogDebug($"Interstitial video ad not ready for ad unit ID: {adUnitId}, placement: {placement}");
            return AdRequestResults.NotReady;
        }
        /// <summary>
        /// Attempts to show an interstitial video ad for the specified placement.
        /// </summary>
        /// <param name="placement">The name of the ad placement.</param>
        /// <returns>The result of the ad request, indicating whether the interstitial video ad was shown or not.</returns>

        public AdRequestResults ShowInterstitialVideo(string placement)
        {
            Logger.LogDebug($"ShowInterstitialVideo started for placement: {placement}");
            var adRequestResults = AdRequestResults.Successful;
            if (!isInitialled)
            {
                Logger.LogError($"ShowInterstitialVideo: The advertising SDK is not initialized.");
                return adRequestResults;
            }
            var adUnitId = adPlayingStrategy.GetInterstitialVideoAdUnitId(placement, out adRequestResults);
            if (string.IsNullOrEmpty(adUnitId))
            {
                Logger.LogWarning($"No ad unit ID found for interstitial video, placement: {placement}");
                return adRequestResults;
            }
            if (InterstitialVideoPlayer.HasInterstitialVideo(adUnitId))
            {
                Logger.LogDebug($"Show interstitial video for ad unit ID: {adUnitId}, placement: {placement}");
                InterstitialVideoPlayer.ShowInterstitialVideo(adUnitId, placement);
                return adRequestResults;
            }
            adRequestResults = AdRequestResults.NotReady;
            Logger.LogWarning($"Interstitial video ad not ready, requesting ad for ad unit ID: {adUnitId}, placement: {placement}");
            InterstitialVideoPlayer.RequestInterstitialVideo(adUnitId);
            return adRequestResults;
        }
        /// <summary>
        /// Requests an interstitial video ad for the specified placement.
        /// </summary>
        /// <param name="placement">The name of the ad placement.</param>
        public void RequestInterstitialVideo(string placement = "")
        {
            Logger.LogDebug($"RequestInterstitialVideo started for placement: {placement}");

            var adUnitId = adPlayingStrategy.GetInterstitialVideoAdUnitId(placement, out AdRequestResults adRequestResults);

            if (string.IsNullOrEmpty(adUnitId))
            {
                Logger.LogWarning($"No ad unit ID found for interstitial, placement: {placement}, results: {adRequestResults}");
                return;
            }

            if (!InterstitialVideoPlayer.HasInterstitialVideo(adUnitId))
            {
                Logger.LogDebug($"Requesting interstitial video for ad unit ID: {adUnitId}, placement: {placement}");
                InterstitialVideoPlayer.RequestInterstitialVideo(adUnitId);
            }
        }

        public void ShowBanner(string placement = "")
        {
            if (!isInitialled)
            {
                Logger.LogError($"ShowBanner: The advertising SDK is not initialized.");
                return;
            }
            var adUnitId = adPlayingStrategy.GetBannerAdUnitId(placement, out AdRequestResults adRequestResults);
            if (string.IsNullOrEmpty(adUnitId))
            {
                Logger.LogError("Has no ad unit id for banner.");
                return;
            }
            BannerPlayer.ShowBanner(adUnitId);
        }
        public void HideBanner(string placement = "")
        {
            if (!isInitialled)
            {
                Logger.LogError($"HideBanner: The advertising SDK is not initialized.");
                return;
            }
            var adUnitId = adPlayingStrategy.GetBannerAdUnitId(placement, out AdRequestResults adRequestResults);
            if (string.IsNullOrEmpty(adUnitId))
            {
                Logger.LogError("Has no ad unit id for banner.");
                return;
            }
            BannerPlayer.HideBanner(adUnitId);
        }
    }
}
