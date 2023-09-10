namespace SuuchaStudio.Unity.Core.AdPlaying
{
#pragma warning disable CS0067
    public class EmptyRewardedVideoPlayer : SuuchaBase, IRewardedVideoPlayer
    {

        public event RewardedVideoClickedHandler OnClicked;
        public event RewardedVideoClosedHandler OnClosed;
        public event RewardedVideoShownHandler OnShown;
        public event RewardedVideoLoadedHandler OnLoaded;
        public event RewardedVideoRevenuePaidHandler OnRevenuePaid;
        public event RewardedVideoLoadFailedHandler OnLoadFailed;
        public event RewardedVideoShowFailedHandler OnShowFailed;
        public event RewardedVideoCompletedHandler OnCompleted;

        private static IRewardedVideoPlayer rewardedVideoPlayer;

        public static IRewardedVideoPlayer Instance
        {
            get
            {
                rewardedVideoPlayer ??= new EmptyRewardedVideoPlayer();
                return rewardedVideoPlayer;
            }
        }
        public EmptyRewardedVideoPlayer()
        {

        }
        public bool HasRewardedVideo(string adUnitId)
        {
            Logger.LogError($"Has no rewared video player, please init.");
            return false;
        }

        public void RequestRewardedVideo(string adUnitId)
        {
            Logger.LogError($"Has no rewared video player, please init.");
        }

        public void ShowRewardedVideo(string adUnitId, string placement)
        {
            Logger.LogError($"Has no rewared video player, please init.");
        }
    }
}
