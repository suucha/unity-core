using SuuchaStudio.Unity.Core.AdPlaying;
using System.Collections.Generic;

namespace SuuchaStudio.Unity.Core
{
    public partial class Suucha
    {
        private SuuchaAdPlaying suuchaAdPlaying;
        /// <summary>
        /// Enables the ad playing.
        /// </summary>
        /// <param name="adPlayerManager">The ad player manager.</param>
        /// <param name="adPlayingStrategy">The ad playing strategy.</param>
        /// <returns></returns>
        public Suucha EnableAdPlaying(IAdPlayerManager adPlayerManager, IAdPlayingStrategy adPlayingStrategy)
        {
            suuchaAdPlaying = new SuuchaAdPlaying(adPlayerManager, adPlayingStrategy);
            return this;
        }
        /// <summary>
        /// Enables the ad playing event.
        /// </summary>
        /// <param name="enablePlacementEvents">The enable placement events.</param>
        /// <returns></returns>
        public Suucha EnableAdPlayingEvent(List<AdPlayingEnableEvents> enableEvents = null, List<AdPlayingEnableEvents> enablePlacementEvents = null)
        {
            if(suuchaAdPlaying == null)
            {
                throw new System.InvalidOperationException("Please call EnableAdPlaying first.");
            }
            suuchaAdPlaying.EnableAdPlayingEvent(enableEvents, enablePlacementEvents);
            return this;
        }
        /// <summary>
        /// Determines whether [has rewarded video] [the specified placement].
        /// </summary>
        /// <param name="placement">The placement.</param>
        /// <returns></returns>
        public AdRequestResults HasRewardedVideo(string placement)
        {
            if (suuchaAdPlaying == null)
            {
                throw new System.InvalidOperationException("Please call EnableAdPlaying first.");
            }
            return suuchaAdPlaying.HasRewardedVideo(placement);
        }
        /// <summary>
        /// Shows the rewarded video.
        /// </summary>
        /// <param name="placement">The placement.</param>
        /// <returns></returns>
        public AdRequestResults ShowRewardedVideo(string placement)
        {
            if (suuchaAdPlaying == null)
            {
                throw new System.InvalidOperationException("Please call EnableAdPlaying first.");
            }
            return suuchaAdPlaying.ShowRewardedVideo(placement);
        }
        /// <summary>
        /// Requests the rewarded video.
        /// </summary>
        /// <param name="placement">The placement.</param>
        public void RequestRewardedVideo(string placement)
        {
            if (suuchaAdPlaying == null)
            {
                throw new System.InvalidOperationException("Please call EnableAdPlaying first.");
            }
            suuchaAdPlaying.RequestRewardedVideo(placement);
        }
        /// <summary>
        /// Determines whether [has interstitial video] [the specified placement].
        /// </summary>
        /// <param name="placement">The placement.</param>
        /// <returns></returns>
        public AdRequestResults HasInterstitialVideo(string placement)
        {
            if (suuchaAdPlaying == null)
            {
                throw new System.InvalidOperationException("Please call EnableAdPlaying first.");
            }
            return suuchaAdPlaying.HasInterstitialVideo(placement);
        }
        /// <summary>
        /// Shows the interstitial video.
        /// </summary>
        /// <param name="placement">The placement.</param>
        /// <returns></returns>
        public AdRequestResults ShowInterstitialVideo(string placement)
        {
            if (suuchaAdPlaying == null)
            {
                throw new System.InvalidOperationException("Please call EnableAdPlaying first.");
            }
            return suuchaAdPlaying.ShowInterstitialVideo(placement);
        }
        /// <summary>
        /// Requests the interstitial video.
        /// </summary>
        /// <param name="placement">The placement.</param>
        public void RequestInterstitialVideo(string placement = "")
        {
            if (suuchaAdPlaying == null)
            {
                throw new System.InvalidOperationException("Please call EnableAdPlaying first.");
            }
            suuchaAdPlaying.RequestInterstitialVideo(placement);
        }

        /// <summary>
        /// Shows the banner.
        /// </summary>
        /// <param name="placement">The placement.</param>
        public void ShowBanner(string placement = "")
        {
            if (suuchaAdPlaying == null)
            {
                throw new System.InvalidOperationException("Please call EnableAdPlaying first.");
            }
            suuchaAdPlaying.ShowBanner(placement);
        }
        /// <summary>
        /// Hides the banner.
        /// </summary>
        /// <param name="placement">The placement.</param>
        public void HideBanner(string placement = "")
        {
            if (suuchaAdPlaying == null)
            {
                throw new System.InvalidOperationException("Please call EnableAdPlaying first.");
            }
            suuchaAdPlaying.HideBanner(placement);
        }
    }
}
