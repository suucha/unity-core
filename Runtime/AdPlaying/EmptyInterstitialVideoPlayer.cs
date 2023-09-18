namespace SuuchaStudio.Unity.Core.AdPlaying
{
#pragma warning disable CS0067
    public class EmptyInterstitialVideoPlayer : SuuchaBase, IInterstitialVideoPlayer
    {
        public event InterstitialVideoDismissedHandler OnDismissed;
        public event InterstitialVideoLoadedHandler OnLoaded;
        public event InterstitialVideoLoadFailedHandler OnLoadFailed;
        public event InterstitialVideoShownHandler OnShown;
        public event InterstitialVideoClickedHandler OnClicked;
        public event InterstitialVideoRevenuePaidHandler OnRevenuePaid;
        public event InterstitialVideoShowFailedHandler OnShowFailed;
        public event InterstitialVideoCompletedHandler OnCompleted;

        private static IInterstitialVideoPlayer instance;
        public static IInterstitialVideoPlayer Instance
        {
            get
            {
                instance ??= new EmptyInterstitialVideoPlayer();
                return instance;
            }
        }
        public bool HasInterstitialVideo(string adUnitId)
        {
            return false;
        }

        public void RequestInterstitialVideo(string adUnitId)
        {

        }

        public void ShowInterstitialVideo(string adUnitId, string placement)
        {

        }
    }
}
