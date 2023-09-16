using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SuuchaStudio.Unity.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

namespace SuuchaStudio.Unity.Core.AdPlaying
{
    /// <summary>
    /// Implementation of an ad playing strategy that limits the number of times an ad can be shown (MaxTimes strategy).
    /// </summary>
    public class MaxTimesAdPlayingStrategy : AdPlayingStrategyAbstract
    {
        private readonly MaxTimesAdPlayingStrategyConfiguration configuration;
        public override string StrategyName
        {
            get
            {
                return "MaxTimes";
            }
        }
        private readonly Dictionary<string, AdUnitIdRequestInfo> requestAdRetryTimes;
        private readonly Dictionary<string, int> adUnitIdPlayTimes;
        /// <summary>
        /// Initializes a new instance of the MaxTimesAdPlayingStrategy class.
        /// </summary>
        /// <param name="configuration">The configuration for the MaxTimesAdPlayingStrategy.</param>
        public MaxTimesAdPlayingStrategy(MaxTimesAdPlayingStrategyConfiguration configuration)
        {
            this.configuration = configuration;
            requestAdRetryTimes = new Dictionary<string, AdUnitIdRequestInfo>();
            adUnitIdPlayTimes = new Dictionary<string, int>();

        }
        PlayTimesDb PlayTimes
        {
            get
            {
                var date = new DateTime(Suucha.App.GetRemoteUtcTimestamp());
                var dateKey = date.ToUniversalTime().ToString("yyyy-MM-dd");
                var json = LocalStorage.GetString(dateKey, "");
                if (string.IsNullOrEmpty(json))
                {
                    json = JsonConvert.SerializeObject(new PlayTimesDb(dateKey));
                    LocalStorage.SetString(dateKey, json);
                }
                var times = new PlayTimesDb(dateKey);
                try
                {
                    times = JsonConvert.DeserializeObject<PlayTimesDb>(json);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Get play times error:{ex.Message}");
                    LocalStorage.SetString(dateKey, JsonConvert.SerializeObject(times));
                }
                return times;
            }
            set
            {
                if (value == null)
                {
                    return;
                }
                LocalStorage.SetString(value.Date, JsonConvert.SerializeObject(value));
            }
        }
        /// <summary>
        /// Gets the rewarded video ad unit ID based on the MaxTimes strategy.
        /// </summary>
        /// <param name="placement">The name of the ad placement.</param>
        /// <param name="adGetResults">An output parameter to indicate the result of the ad request.</param>
        /// <returns>The ad unit ID for the rewarded video, or null if not available.</returns>
        public override string GetRewardedVideoAdUnitId(string placement, out AdRequestResults adGetResults)
        {
            Logger.LogDebug($"GetRewardedVideoAdUnitId started for placement: {placement}");
            adGetResults = AdRequestResults.Successful;
            var adUnit = configuration.AdUnits.Where(ad => ad.AdType == AdType.Rewarded).OrderBy(ad => ad.Priority).FirstOrDefault();
            if (adUnit == null)
            {
                adGetResults = AdRequestResults.InvalidPlacement;
                Logger.LogWarning($"No ad unit ID found for rewarded, placement: {placement}");
                return null;
            }
            if (PlayTimes.IsMaxTimes(configuration, AdType.Rewarded, adUnit.AdUnitId, Logger))
            {
                adGetResults = AdRequestResults.MaxTimes;
                Logger.LogDebug($"Max play times reached for rewarded, ad unit ID: {adUnit.AdUnitId}, placement: {placement}");
                return null;
            }
            Logger.LogDebug($"GetRewardedVideoAdUnitId completed for placement: {placement}, ad unit ID: {adUnit.AdUnitId}");
            return adUnit.AdUnitId;
        }
        /// <summary>
        /// Gets the ad unit ID for interstitial video based on the MaxTimes strategy.
        /// </summary>
        /// <param name="placement">The name of the ad placement.</param>
        /// <param name="adGetResults">An output parameter to indicate the result of the ad request.</param>
        /// <returns>The ad unit ID for the interstitial video, or null if not available.</returns>
        public override string GetInterstitialVideoAdUnitId(string placement, out AdRequestResults adGetResults)
        {
            Logger.LogDebug($"GetInterstitialVideoAdUnitId started for placement: {placement}");
            adGetResults = AdRequestResults.Successful;
            var adUnit = configuration.AdUnits.Where(ad => ad.AdType == AdType.Interstitial).OrderBy(ad => ad.Priority).FirstOrDefault();
            if (adUnit == null)
            {
                adGetResults = AdRequestResults.InvalidPlacement;
                Logger.LogWarning($"No ad unit ID found for interstitial, placement: {placement}");
                return null;
            }
            if (PlayTimes.IsMaxTimes(configuration, AdType.Interstitial, adUnit.AdUnitId, Logger))
            {
                adGetResults = AdRequestResults.MaxTimes;
                Logger.LogDebug($"Max play times reached for interstitial, ad unit ID: {adUnit.AdUnitId}");
                return null;
            }
            Logger.LogDebug($"GetInterstitialVideoAdUnitId completed for placement: {placement}, ad unit ID: {adUnit.AdUnitId}");
            return adUnit.AdUnitId;
        }

        /// <summary>
        /// Gets the ad unit ID for banner ads based on the MaxTimes strategy.
        /// </summary>
        /// <param name="placement">The name of the ad placement.</param>
        /// <param name="adGetResults">An output parameter to indicate the result of the ad request.</param>
        /// <returns>The ad unit ID for banner ads, or null if not available.</returns>
        public override string GetBannerAdUnitId(string placement, out AdRequestResults adGetResults)
        {
            Logger.LogDebug($"GetBannerAdUnitId started for placement: {placement}");
            adGetResults = AdRequestResults.Successful;
            var adUnit = configuration.AdUnits.Where(ad => ad.AdType == AdType.Banner).OrderBy(ad => ad.Priority).FirstOrDefault();
            if (adUnit == null)
            {
                adGetResults = AdRequestResults.InvalidPlacement;
                Logger.LogWarning($"No ad unit ID found for banner, placement: {placement}");
                return null;
            }
            Logger.LogDebug($"GetBannerAdUnitId completed for placement: {placement}, ad unit ID: {adUnit.AdUnitId}");
            return adUnit.AdUnitId;
        }
        private UniTask TimerTimeout(object adUnitId)
        {
            Logger.LogDebug($"TimerTimeout started for ad unit ID: :{adUnitId}");
            if (adUnitId == null)
            {
                Logger.LogError($"Ad unit id can not null.");
                return UniTask.CompletedTask;
            }
            var adUnit = configuration.AdUnits.FirstOrDefault(ad => ad.AdUnitId == adUnitId.ToString());
            if (adUnit == null)
            {
                Logger.LogWarning($"No ad unit found for ID: {adUnitId}");
                return UniTask.CompletedTask;
            }
            if (!requestAdRetryTimes.ContainsKey(adUnit.AdUnitId))
            {
                Logger.LogWarning($"Ad unit ID not in retry list. Ad unit ID: {adUnit.AdUnitId}");
                return UniTask.CompletedTask;
            }
            var retryTimes = requestAdRetryTimes[adUnit.AdUnitId];
            retryTimes.Stop();
            if (retryTimes.RetryTimes >= configuration.RetryIntervals.Count)
            {
                Logger.LogWarning("Retry times reached maximum.");
                requestAdRetryTimes.Remove(adUnit.AdUnitId);
                return UniTask.CompletedTask;
            }
            var peroid = configuration.RetryIntervals[retryTimes.RetryTimes];
            var hasAd = false;
            var adType = adUnit.AdType;
            switch (adType)
            {
                case AdType.Rewarded:
                    hasAd = RewardedVideoPlayer.HasRewardedVideo(adUnit.AdUnitId);
                    if (!hasAd)
                    {
                        Logger.LogInformation($"Retrying request for rewarded video, AdUnitId: {adUnit.AdUnitId}, Retry Count: {retryTimes.RetryTimes}");
                        RewardedVideoPlayer.RequestRewardedVideo(adUnit.AdUnitId);
                    }
                    break;
                case AdType.Interstitial:
                    hasAd = InterstitialVideoPlayer.HasInterstitialVideo(adUnit.AdUnitId);
                    if (!hasAd)
                    {
                        Logger.LogInformation($"Retrying request for interstitial video, AdUnitId: {adUnit.AdUnitId}, Retry Count: {retryTimes.RetryTimes}");
                        InterstitialVideoPlayer.RequestInterstitialVideo(adUnit.AdUnitId);
                    }
                    break;
                case AdType.Banner:
                    break;
            }
            Logger.LogDebug($"TimerTimeout completed for ad unit ID: {adUnitId}");
            return UniTask.CompletedTask;
        }
        #region Rewarded video callbacks
        /// <summary>
        /// Handles the event when showing a rewarded video ad fails.
        /// </summary>
        /// <param name="adUnitId">The AdUnit ID of the failed rewarded video ad.</param>
        /// <param name="error">The error message describing the failure.</param>
        /// <param name="placement">The placement of the ad where the failure occurred.</param>
        protected override void RewardedVideoPlayerOnShowFailed(string adUnitId, string error, string placement)
        {
            Logger.LogDebug($"Rewarded video show failed for AdUnit ID: {adUnitId} @ {placement}, Error: {error}");
            if (requestAdRetryTimes.ContainsKey(adUnitId))
            {
                requestAdRetryTimes.Remove(adUnitId);
            }
            if (!adUnitIdPlayTimes.ContainsKey(adUnitId))
            {
                adUnitIdPlayTimes.Add(adUnitId, 0);
            }
            var playTimes = adUnitIdPlayTimes[adUnitId];
            playTimes++;
            adUnitIdPlayTimes[adUnitId] = playTimes;
            var adUnit = configuration.AdUnits.FirstOrDefault(ad => ad.AdUnitId == adUnitId);
            if (adUnit == null)
            {
                Logger.LogWarning($"No ad unit found for AdUnit ID: {adUnitId}");
                return;
            }
            if (playTimes > adUnit.MaxTimesToPlay)
            {
                Logger.LogDebug($"Max play times reached for AdUnit ID: {adUnitId}");
                return;
            }
            if (!RewardedVideoPlayer.HasRewardedVideo(adUnitId))
            {
                Logger.LogInformation($"Retrying request for rewarded video, AdUnitId: {adUnitId}");
                RewardedVideoPlayer.RequestRewardedVideo(adUnitId);
            }
            Logger.LogDebug($"Rewarded video show failed processing completed for AdUnit ID: {adUnitId} @ {placement}");
        }
        /// <summary>
        /// Handles the event when a rewarded video ad has been successfully loaded.
        /// </summary>
        /// <param name="adUnitId">The AdUnit ID of the loaded rewarded video ad.</param>
        /// <param name="adCallbackInfo">Additional information about the ad loading event.</param>
        protected override void RewardedVideoPlayerOnLoaded(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            Logger.LogDebug($"Rewarded video loaded: {adUnitId} @ {adCallbackInfo.Placement}");
            if (requestAdRetryTimes.ContainsKey(adUnitId))
            {
                requestAdRetryTimes.Remove(adUnitId);
            }
        }

        /// <summary>
        /// Handles the event when a rewarded video ad is shown to the user.
        /// </summary>
        /// <param name="adUnitId">The AdUnit ID of the shown rewarded video ad.</param>
        /// <param name="adCallbackInfo">Additional information about the ad showing event.</param>
        protected override void RewardedVideoPlayerOnShown(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            Logger.LogDebug($"Rewarded video on shown:{adUnitId} @ {adCallbackInfo.Placement}");
            var adUnit = configuration.AdUnits.FirstOrDefault(ad => ad.AdUnitId == adUnitId);
            if (adUnit == null)
            {
                Logger.LogWarning($"No ad unit found for AdUnit ID: {adUnitId}");
                return;
            }
            if (requestAdRetryTimes.ContainsKey(adUnitId))
            {
                requestAdRetryTimes.Remove(adUnitId);
            }
            var playTimes = PlayTimes;
            playTimes.Play(AdType.Rewarded, adUnitId);
            PlayTimes = playTimes;
            if (playTimes.IsMaxTimes(configuration, AdType.Rewarded, adUnitId, Logger))
            {
                Logger.LogDebug($"Max play times reached for AdUnit ID: {adUnitId}");
                return;
            }
            if (!RewardedVideoPlayer.HasRewardedVideo(adUnitId))
            {
                Logger.LogInformation($"Retrying request for rewarded video, AdUnitId: {adUnitId}");
                RewardedVideoPlayer.RequestRewardedVideo(adUnitId);
            }
            Logger.LogDebug($"Rewarded video onshown processing completed for AdUnit ID: {adUnitId} @ {placement}");
        }

        /// <summary>
        /// Handles the event when loading a rewarded video ad fails.
        /// </summary>
        /// <param name="adUnitId">The AdUnit ID of the failed rewarded video ad.</param>
        /// <param name="errorMessage">The error message describing the failure.</param>
        protected override void RewardedVideoPlayerOnFailed(string adUnitId, string errorMessage)
        {
            Logger.LogDebug($"Rewarded video load failed: {adUnitId}, Error: {errorMessage}");
            if (!requestAdRetryTimes.ContainsKey(adUnitId))
            {
                requestAdRetryTimes.Add(adUnitId, new AdUnitIdRequestInfo(adUnitId, TimerTimeout));
            }
            var info = requestAdRetryTimes[adUnitId];

            if (info.RetryTimes >= configuration.RetryIntervals.Count)
            {
                Logger.LogDebug($"Max play times reached for AdUnit ID: {adUnitId}");
                requestAdRetryTimes.Remove(adUnitId);
                return;
            }
            if (info.IsRequesting)
            {
                return;
            }
            Logger.LogDebug($"Retry attempt: {info.RetryTimes}");
            info.SetTimeout(configuration.RetryIntervals[info.RetryTimes]);
        }
        #endregion

        #region Interstitial video callbacks
        /// <summary>
        /// Handles the event when showing an interstitial video ad fails.
        /// </summary>
        /// <param name="adUnitId">The AdUnit ID of the failed interstitial video ad.</param>
        /// <param name="error">The error message describing the failure.</param>
        /// <param name="placement">The placement of the ad where the failure occurred.</param>
        protected override void InterstitialVideoPlayerOnShowFailed(string adUnitId, string error, string placement)
        {
            Logger.LogDebug($"Interstitial video show failed for AdUnit ID: {adUnitId} @ {placement}");
            if (requestAdRetryTimes.ContainsKey(adUnitId))
            {
                requestAdRetryTimes.Remove(adUnitId);
            }
            if (!adUnitIdPlayTimes.ContainsKey(adUnitId))
            {
                adUnitIdPlayTimes.Add(adUnitId, 0);
            }
            var playTimes = adUnitIdPlayTimes[adUnitId];
            playTimes++;
            adUnitIdPlayTimes[adUnitId] = playTimes;
            var adUnit = configuration.AdUnits.FirstOrDefault(ad => ad.AdUnitId == adUnitId);
            if (adUnit == null)
            {
                return;
            }
            if (playTimes > adUnit.MaxTimesToPlay)
            {
                Logger.LogWarning($"PlayTimes exceeded maximum allowed for AdUnit ID: {adUnitId}");
                return;
            }
            if (!InterstitialVideoPlayer.HasInterstitialVideo(adUnitId))
            {
                Logger.LogDebug($"Requesting a new interstitial video for AdUnit ID: {adUnitId}");
                InterstitialVideoPlayer.RequestInterstitialVideo(adUnitId);
            }
        }
        /// <summary>
        /// Handles the event when an interstitial video ad has successfully loaded.
        /// </summary>
        /// <param name="adUnitId">The AdUnit ID of the loaded interstitial video ad.</param>
        /// <param name="adCallbackInfo">Additional callback information.</param>
        protected override void InterstitialVideoPlayerOnLoaded(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            Logger.LogDebug($"Interstitial video loaded for AdUnit ID: {adUnitId} @ {adCallbackInfo.Placement}");
            if (requestAdRetryTimes.ContainsKey(adUnitId))
            {
                requestAdRetryTimes.Remove(adUnitId);
                Logger.LogDebug($"Removed AdUnit ID: {adUnitId} from retry list.");
            }
        }

        /// <summary>
        /// Handles the event when an interstitial video ad is shown.
        /// </summary>
        /// <param name="adUnitId">The AdUnit ID of the shown interstitial video ad.</param>
        /// <param name="adCallbackInfo">Additional callback information.</param>
        protected override void InterstitialVideoPlayerOnShown(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            Logger.LogDebug($"Interstitial video shown for AdUnit ID: {adUnitId} @ {adCallbackInfo.Placement}");
            var adUnit = configuration.AdUnits.FirstOrDefault(ad => ad.AdUnitId == adUnitId);
            if (adUnit == null)
            {
                Logger.LogError($"No ad unit ID found: {adUnitId} @ {adCallbackInfo.Placement}");
                return;
            }
            if (requestAdRetryTimes.ContainsKey(adUnitId))
            {
                requestAdRetryTimes.Remove(adUnitId);
                Logger.LogDebug($"Removed AdUnit ID: {adUnitId} from retry list.");
            }
            var playTimes = PlayTimes;
            playTimes.Play(AdType.Interstitial, adUnitId);
            PlayTimes = playTimes;
            if (playTimes.IsMaxTimes(configuration, AdType.Interstitial, adUnitId, Logger))
            {
                Logger.LogWarning($"PlayTimes exceeded maximum allowed for AdUnit ID: {adUnitId} @ {adCallbackInfo.Placement}");
                return;
            }
            if (!InterstitialVideoPlayer.HasInterstitialVideo(adUnitId))
            {
                Logger.LogDebug($"Requesting a new interstitial video for AdUnit ID: {adUnitId}");
                InterstitialVideoPlayer.RequestInterstitialVideo(adUnitId);
            }
        }

        /// <summary>
        /// Handles the event when loading an interstitial video ad fails.
        /// </summary>
        /// <param name="adUnitId">The AdUnit ID for which the ad loading failed.</param>
        /// <param name="errorMessage">The error message describing the failure.</param>
        protected override void InterstitialVideoPlayerOnLoadFailed(string adUnitId, string errorMessage)
        {
            Logger.LogDebug($"Interstitial video load failed for AdUnit ID: {adUnitId}, Error: {errorMessage}");
            if (!requestAdRetryTimes.ContainsKey(adUnitId))
            {
                requestAdRetryTimes.Add(adUnitId, new AdUnitIdRequestInfo(adUnitId, TimerTimeout));
                Logger.LogDebug($"Added AdUnit ID: {adUnitId} to retry list with TimerTimeout callback.");
            }
            var info = requestAdRetryTimes[adUnitId];

            if (info.RetryTimes >= configuration.RetryIntervals.Count)
            {
                Logger.LogDebug($"Maximum retry times reached for AdUnit ID: {adUnitId}");
                requestAdRetryTimes.Remove(adUnitId);
                return;
            }
            Logger.LogDebug($"Retry attempt #{info.RetryTimes}");
            info.SetTimeout(configuration.RetryIntervals[info.RetryTimes]);
        }
        #endregion
    }

    class AdUnitIdRequestInfo
    {
        private readonly Func<string, UniTask> callback;

        public AdUnitIdRequestInfo(string adUnitId, Func<string, UniTask> callback)
        {
            this.callback = callback;
            AdUnitId = adUnitId;
            RetryTimes = 0;
            LastRetryTime = DateTime.Now;
        }

        public string AdUnitId { get; private set; }
        public int RetryTimes { get; set; }
        public DateTime LastRetryTime { get; set; }
        public bool IsRequesting { get; set; }
        public void SetTimeout(int period)
        {
            RetryTimes++;
            TimesTaskManager.AddTask<string>(period, period, 1, callback, AdUnitId);
            IsRequesting = true;
        }
        public void Stop()
        {
            IsRequesting = false;
        }
        public void Reset()
        {
            RetryTimes = 0;
            IsRequesting = false;
        }
    }
    class PlayTimesDb
    {
        public PlayTimesDb(string date)
        {
            Date = date;
            AdUnitPlayTimes = new Dictionary<string, int>();
        }
        public string Date { get; set; }
        public int PlayTimes { get; set; }
        public Dictionary<string, int> AdUnitPlayTimes { get; set; }
        public int RewardedVideoPlayTimes { get; set; }
        public int InterstitialVideoPlayTimes { get; set; }
        public void Play(AdType adType, string adUnitId)
        {
            if (!AdUnitPlayTimes.ContainsKey(adUnitId))
            {
                AdUnitPlayTimes.Add(adUnitId, 0);
            }
            AdUnitPlayTimes[adUnitId]++;
            if (adType == AdType.Rewarded)
            {
                RewardedVideoPlayTimes++;
            }
            if (adType == AdType.Interstitial)
            {
                InterstitialVideoPlayTimes++;
            }
            PlayTimes++;
        }
        public int GetPlayTimes(string adUnitId)
        {
            if (!AdUnitPlayTimes.ContainsKey(adUnitId))
            {
                return 1;
            }
            return AdUnitPlayTimes[adUnitId];
        }
        /// <summary>
        /// Checks if the specified ad type and AdUnit ID have reached their maximum allowed play times based on the provided configuration.
        /// </summary>
        /// <param name="configuration">The configuration that defines maximum play times for different ad types and AdUnit IDs.</param>
        /// <param name="adType">The type of ad (e.g., Rewarded, Interstitial).</param>
        /// <param name="adUnitId">The AdUnit ID of the ad being checked.</param>
        /// <param name="logger">The logger for logging messages.</param>
        /// <returns>True if the maximum play times have been reached; otherwise, false.</returns>

        public bool IsMaxTimes(MaxTimesAdPlayingStrategyConfiguration configuration, AdType adType, string adUnitId, ILogger logger)
        {
            if (configuration.MaxTimes != 0 && PlayTimes > configuration.MaxTimes)
            {
                logger.LogError($"Max times reached for strategy: {configuration.MaxTimes}, play time: {PlayTimes}");
                return true;
            }
            if (adType == AdType.Interstitial && configuration.InterstitialMaxTimes != 0 && InterstitialVideoPlayTimes > configuration.InterstitialMaxTimes)
            {
                logger.LogError($"Max times reached for InterstitialVideo: {configuration.InterstitialMaxTimes}, play time: {InterstitialVideoPlayTimes}");
                return true;
            }
            if (adType == AdType.Rewarded && configuration.RewardedMaxTimes != 0 && RewardedVideoPlayTimes > configuration.RewardedMaxTimes)
            {
                logger.LogError($"Max times reached for RewardedVideo: {configuration.RewardedMaxTimes}, play time: {RewardedVideoPlayTimes}");
                return true;
            }
            var adUnitMaxTimes = configuration.GetMaxTimes(adUnitId);
            var adUnitPlayTimes = GetPlayTimes(adUnitId);
            if (adUnitMaxTimes != 0 && adUnitPlayTimes > adUnitMaxTimes)
            {
                logger.LogError($"Max times reached for Ad unit: {adUnitMaxTimes}, play times: {adUnitPlayTimes}");
                return true;
            }
            return false;
        }
    }
}

