using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SuuchaStudio.Unity.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SuuchaStudio.Unity.Core.AdPlaying
{
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
        public override string GetRewardedVideoAdUnitId(string placement, out AdRequestResults adGetResults)
        {
            adGetResults = AdRequestResults.Successful;
            var adUnit = configuration.AdUnits.Where(ad => ad.AdType == AdType.Rewarded).OrderBy(ad => ad.Priority).FirstOrDefault();
            if (adUnit == null)
            {
                adGetResults = AdRequestResults.InvalidPlacement;
                Logger.LogWarning($"No ad unit id found.");
                return null;
            }
            if (PlayTimes.IsMaxTimes(configuration, AdType.Rewarded, adUnit.AdUnitId, Logger))
            {
                adGetResults = AdRequestResults.MaxTimes;
                return null;
            }

            return adUnit.AdUnitId;
        }
        public override string GetInterstitialVideoAdUnitId(string placement, out AdRequestResults adGetResults)
        {
            adGetResults = AdRequestResults.Successful;
            var adUnit = configuration.AdUnits.Where(ad => ad.AdType == AdType.Interstitial).OrderBy(ad => ad.Priority).FirstOrDefault();
            if (adUnit == null)
            {
                adGetResults = AdRequestResults.InvalidPlacement;
                Logger.LogWarning($"No ad unit id found.");
                return null;
            }
            if (PlayTimes.IsMaxTimes(configuration, AdType.Interstitial, adUnit.AdUnitId, Logger))
            {
                adGetResults = AdRequestResults.MaxTimes;
                return null;
            }
            return adUnit.AdUnitId;
        }

        public override string GetBannerAdUnitId(string placement, out AdRequestResults adGetResults)
        {
            adGetResults = AdRequestResults.Successful;
            var adUnit = configuration.AdUnits.Where(ad => ad.AdType == AdType.Banner).OrderBy(ad => ad.Priority).FirstOrDefault();
            if (adUnit == null)
            {
                adGetResults = AdRequestResults.InvalidPlacement;
                Logger.LogWarning($"No ad unit id found.");
                return null;
            }
            return adUnit.AdUnitId;
        }
        private UniTask TimerTimeout(object adUnitId)
        {
            Logger.LogDebug($"Strategy: Timer callback:{adUnitId}");
            if (adUnitId == null)
            {
                Logger.LogError($"Strategy: ad unit id can not null.");
                return UniTask.CompletedTask;
            }
            var adUnit = configuration.AdUnits.FirstOrDefault(ad => ad.AdUnitId == adUnitId.ToString());
            if (adUnit == null)
            {
                Logger.LogError($"Strategy: Not found ad unit id:{adUnitId}");
                return UniTask.CompletedTask;
            }
            if (!requestAdRetryTimes.ContainsKey(adUnit.AdUnitId))
            {
                Logger.LogInformation($"Ad unit id not in retry list. ad unit id:{adUnit.AdUnitId}");
                return UniTask.CompletedTask;
            }
            var retryTimes = requestAdRetryTimes[adUnit.AdUnitId];
            retryTimes.Stop();
            if (retryTimes.RetryTimes >= configuration.RetryIntervals.Count)
            {
                Logger.LogInformation($"Strategy: Retry times to max.");
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
                        Logger.LogInformation($"Strategy: Retry request rewarded video, AdUnitId:{adUnit.AdUnitId}, Times:{retryTimes.RetryTimes}");
                        RewardedVideoPlayer.RequestRewardedVideo(adUnit.AdUnitId);
                    }
                    break;
                case AdType.Interstitial:
                    hasAd = InterstitialVideoPlayer.HasInterstitialVideo(adUnit.AdUnitId);
                    if (!hasAd)
                    {
                        Logger.LogInformation($"Strategy: Retry request interstitial video, AdUnitId:{adUnit.AdUnitId}, Times:{retryTimes.RetryTimes}");
                        InterstitialVideoPlayer.RequestInterstitialVideo(adUnit.AdUnitId);
                    }
                    break;
                case AdType.Banner:
                    break;
            }
            return UniTask.CompletedTask;
        }
        #region Rewarded video callbacks
        protected override void RewardedVideoPlayerOnShowFailed(string adUnitId, string error, string placement)
        {
            Logger.LogDebug($"Strategy: Rewarded video on show failed:{adUnitId} @ {placement}");
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
                return;
            }
            if (!RewardedVideoPlayer.HasRewardedVideo(adUnitId))
            {
                RewardedVideoPlayer.RequestRewardedVideo(adUnitId);
            }
        }
        protected override void RewardedVideoPlayerOnLoaded(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            Logger.LogDebug($"Strategy: Rewarded video loaded:{adUnitId} @ {adCallbackInfo.Placement}");
            if (requestAdRetryTimes.ContainsKey(adUnitId))
            {
                requestAdRetryTimes.Remove(adUnitId);
            }
        }

        protected override void RewardedVideoPlayerOnShown(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            Logger.LogDebug($"Strategy: Rewarded video on shown:{adUnitId} @ {adCallbackInfo.Placement}");
            var adUnit = configuration.AdUnits.FirstOrDefault(ad => ad.AdUnitId == adUnitId);
            if (adUnit == null)
            {
                Logger.LogError($"Have no ad unit id:{adUnitId}");
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
                return;
            }
            RewardedVideoPlayer.RequestRewardedVideo(adUnitId);
        }

        protected override void RewardedVideoPlayerOnFailed(string adUnitId, string errorMessage)
        {
            Logger.LogDebug($"Strategy: Rewarded video load failed:{adUnitId}");
            if (!requestAdRetryTimes.ContainsKey(adUnitId))
            {
                requestAdRetryTimes.Add(adUnitId, new AdUnitIdRequestInfo(adUnitId, TimerTimeout));
            }
            var info = requestAdRetryTimes[adUnitId];

            if (info.RetryTimes >= configuration.RetryIntervals.Count)
            {
                Logger.LogDebug($"Retry max times.");
                requestAdRetryTimes.Remove(adUnitId);
                return;
            }
            if (info.IsRequesting)
            {
                return;
            }
            Logger.LogDebug($"Strategy: retry times:{info.RetryTimes}");
            info.SetTimeout(configuration.RetryIntervals[info.RetryTimes]);
        }
        #endregion

        #region Interstitial video callbacks
        protected override void InterstitialVideoPlayerOnShowFailed(string adUnitId, string error, string placement)
        {
            Logger.LogDebug($"Strategy: Interstitial video on shown:{adUnitId} @ {placement}");
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
                return;
            }
            if (!InterstitialVideoPlayer.HasInterstitialVideo(adUnitId))
            {
                InterstitialVideoPlayer.RequestInterstitialVideo(adUnitId);
            }
        }
        protected override void InterstitialVideoPlayerOnLoaded(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            Logger.LogDebug($"Strategy: Interstitial video loaded:{adUnitId} @ {adCallbackInfo.Placement}");
            if (requestAdRetryTimes.ContainsKey(adUnitId))
            {
                requestAdRetryTimes.Remove(adUnitId);
            }
        }

        protected override void InterstitialVideoPlayerOnShown(string adUnitId, AdCallbackInfo adCallbackInfo)
        {
            Logger.LogDebug($"Strategy: Interstitial video on shown:{adUnitId} @ {adCallbackInfo.Placement}");
            var adUnit = configuration.AdUnits.FirstOrDefault(ad => ad.AdUnitId == adUnitId);
            if (adUnit == null)
            {
                Logger.LogError($"Have no ad unit id:{adUnitId}");
                return;
            }
            if (requestAdRetryTimes.ContainsKey(adUnitId))
            {
                requestAdRetryTimes.Remove(adUnitId);
            }
            var playTimes = PlayTimes;
            playTimes.Play(AdType.Interstitial, adUnitId);
            PlayTimes = playTimes;
            if (playTimes.IsMaxTimes(configuration, AdType.Interstitial, adUnitId, Logger))
            {
                return;
            }
            InterstitialVideoPlayer.RequestInterstitialVideo(adUnitId);
        }

        protected override void InterstitialVideoPlayerOnLoadFailed(string adUnitId, string errorMessage)
        {
            Logger.LogDebug($"Strategy: Interstitial video load failed:{adUnitId}");
            if (!requestAdRetryTimes.ContainsKey(adUnitId))
            {
                requestAdRetryTimes.Add(adUnitId, new AdUnitIdRequestInfo(adUnitId, TimerTimeout));
            }
            var info = requestAdRetryTimes[adUnitId];

            if (info.RetryTimes >= configuration.RetryIntervals.Count)
            {
                Logger.LogDebug($"Retry max times.");
                requestAdRetryTimes.Remove(adUnitId);
                return;
            }
            Logger.LogDebug($"Strategy: retry times:{info.RetryTimes}");
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
        public bool IsMaxTimes(MaxTimesAdPlayingStrategyConfiguration configuration, AdType adType, string adUnitId, ILogger logger)
        {
            if (configuration.MaxTimes != 0 && PlayTimes > configuration.MaxTimes)
            {
                logger.LogError($"To max times: {configuration.MaxTimes}, play time: {PlayTimes}");
                return true;
            }
            if (adType == AdType.Interstitial && configuration.InterstitialMaxTimes != 0 && InterstitialVideoPlayTimes > configuration.InterstitialMaxTimes)
            {
                logger.LogError($"InterstitialVideo to max times: {configuration.InterstitialMaxTimes}, play time: {InterstitialVideoPlayTimes}");
                return true;
            }
            if (adType == AdType.Rewarded && configuration.RewardedMaxTimes != 0 && RewardedVideoPlayTimes > configuration.RewardedMaxTimes)
            {
                logger.LogError($"RewardedVideo to max times: {configuration.RewardedMaxTimes}, play time: {RewardedVideoPlayTimes}");
                return true;
            }
            var adUnitMaxTimes = configuration.GetMaxTimes(adUnitId);
            var adUnitPlayTimes = GetPlayTimes(adUnitId);
            if (adUnitMaxTimes != 0 && adUnitPlayTimes > adUnitMaxTimes)
            {
                logger.LogError($"Ad unit play to max times: {adUnitMaxTimes}, play times: {adUnitPlayTimes}");
                return true;
            }
            return false;
        }
    }
}

