namespace SuuchaStudio.Unity.Core.AdPlaying
{
    public class SuuchaAdPlayingCallbacks
    {
        internal static RewardedVideoClickedHandler RewardedVideoClickedHandler;
        internal static RewardedVideoClosedHandler RewardedVideoClosedHandler;
        internal static RewardedVideoShownHandler RewardedVideoShownHandler;
        internal static RewardedVideoLoadedHandler RewardedVideoLoadedHandler;
        internal static RewardedVideoLoadFailedHandler RewardedVideoFailedHandler;
        internal static RewardedVideoShowFailedHandler RewardedVideoShowFailedHandler;
        internal static RewardedVideoRevenuePaidHandler RewardedVideoRevenuePaidHandler;
        internal static RewardedVideoCompletedHandler RewardedVideoCompletedHandler;

        internal static InterstitialVideoDismissedHandler InterstitialVideoDismissedHandler;
        internal static InterstitialVideoLoadedHandler InterstitialVideoLoadedHandler;
        internal static InterstitialVideoLoadFailedHandler InterstitialVideoLoadFailedHandler;
        internal static InterstitialVideoShownHandler InterstitialVideoShownHandler;
        internal static InterstitialVideoShowFailedHandler InterstitialVideoShowFailedHandler;
        internal static InterstitialVideoClickedHandler InterstitialVideoClickedHandler;
        internal static InterstitialVideoRevenuePaidHandler InterstitialVideoRevenuePaidHandler;
        internal static InterstitialVideoCompletedHandler InterstitialVideoCompletedHandler;

        internal static BannerLoadedHandler BannerLoadedHandler;
        internal static BannerFailedHandler BannerFailedHandler;
        internal static BannerClickedHandler BannerClickedHandler;
        internal static BannerRevenuePaidHandler BannerRevenuePaidHandler;

        public static event RewardedVideoClickedHandler OnRewardedVideoClicked
        {
            add
            {
                RewardedVideoClickedHandler += value;
            }
            remove
            {
                RewardedVideoClickedHandler -= value;
            }
        }
        public static event RewardedVideoClosedHandler OnRewardedVideoClosed
        {
            add
            {
                RewardedVideoClosedHandler += value;
            }
            remove
            {
                RewardedVideoClosedHandler -= value;
            }
        }
        public static event RewardedVideoShownHandler OnRewardedVideoShown
        {
            add
            {
                RewardedVideoShownHandler += value;
            }
            remove
            {
                RewardedVideoShownHandler -= value;
            }
        }
        public static event RewardedVideoLoadedHandler OnRewardedVideoLoaded
        {
            add
            {
                RewardedVideoLoadedHandler += value;
            }
            remove
            {
                RewardedVideoLoadedHandler -= value;
            }
        }
        public static event RewardedVideoLoadFailedHandler OnRewardedVideoLoadFailed
        {
            add
            {
                RewardedVideoFailedHandler += value;
            }
            remove
            {
                RewardedVideoFailedHandler -= value;
            }
        }
        public static event RewardedVideoShowFailedHandler OnRewardedVideoShowFailed
        {
            add
            {
                RewardedVideoShowFailedHandler += value;
            }
            remove
            {
                RewardedVideoShowFailedHandler -= value;
            }
        }
        public static event RewardedVideoRevenuePaidHandler OnRewardedVideoRevenuePaid
        {
            add
            {
                RewardedVideoRevenuePaidHandler += value;
            }
            remove
            {
                RewardedVideoRevenuePaidHandler -= value;
            }
        }
        public static event RewardedVideoCompletedHandler OnRewardedVideoCompleted
        {
            add
            {
                RewardedVideoCompletedHandler += value;
            }
            remove
            {
                RewardedVideoCompletedHandler -= value;
            }
        }

        public static event InterstitialVideoDismissedHandler OnInterstitialVideoDismissed
        {
            add
            {
                InterstitialVideoDismissedHandler += value;
            }
            remove
            {
                InterstitialVideoDismissedHandler -= value;
            }
        }
        public static event InterstitialVideoLoadedHandler OnInterstitialVideoLoaded
        {
            add
            {
                InterstitialVideoLoadedHandler += value;
            }
            remove
            {
                InterstitialVideoLoadedHandler -= value;
            }
        }
        public static event InterstitialVideoLoadFailedHandler OnInterstitialVideoLoadFailed
        {
            add
            {
                InterstitialVideoLoadFailedHandler += value;
            }
            remove
            {
                InterstitialVideoLoadFailedHandler -= value;
            }
        }
        public static event InterstitialVideoShownHandler OnInterstitialVideoShown
        {
            add
            {
                InterstitialVideoShownHandler += value;
            }
            remove
            {
                InterstitialVideoShownHandler -= value;
            }
        }
        public static event InterstitialVideoClickedHandler OnInterstitialVideoClicked
        {
            add
            {
                InterstitialVideoClickedHandler += value;
            }
            remove
            {
                InterstitialVideoClickedHandler -= value;
            }
        }
        public static event InterstitialVideoShowFailedHandler OnInterstitialVideoShowFailed
        {
            add
            {
                InterstitialVideoShowFailedHandler += value;
            }
            remove
            {
                InterstitialVideoShowFailedHandler -= value;
            }
        }
        public static event InterstitialVideoRevenuePaidHandler OnInterstitialVideoRevenuePaid
        {
            add
            {
                InterstitialVideoRevenuePaidHandler += value;
            }
            remove
            {
                InterstitialVideoRevenuePaidHandler -= value;
            }
        }
        public static event InterstitialVideoCompletedHandler OnInterstitialVideoCompleted
        {
            add
            {
                InterstitialVideoCompletedHandler += value;
            }
            remove
            {
                InterstitialVideoCompletedHandler -= value;
            }
        }

        public static event BannerRevenuePaidHandler OnBannerRevenuePaid
        {
            add
            {
                BannerRevenuePaidHandler += value;
            }
            remove
            {
                BannerRevenuePaidHandler -= value;
            }
        }
        public static event BannerLoadedHandler OnBannerLoaded
        {
            add
            {
                BannerLoadedHandler += value;
            }
            remove
            {
                BannerLoadedHandler -= value;
            }
        }
        public static event BannerFailedHandler OnBannerFailed
        {
            add
            {
                BannerFailedHandler += value;
            }
            remove
            {
                BannerFailedHandler -= value;
            }
        }
        public static event BannerClickedHandler OnBannerClicked
        {
            add
            {
                BannerClickedHandler += value;
            }
            remove
            {
                BannerClickedHandler -= value;
            }
        }
    }
}
