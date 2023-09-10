namespace SuuchaStudio.Unity.Core.AdPlaying
{
    public delegate void InterstitialVideoDismissedHandler(string adUnitId, AdCallbackInfo adCallbackInfo);
    public delegate void InterstitialVideoLoadedHandler(string adUnitId, AdCallbackInfo adCallbackInfo);
    public delegate void InterstitialVideoLoadFailedHandler(string adUnitId, string errorMessage);
    public delegate void InterstitialVideoShownHandler(string adUnitId, AdCallbackInfo adCallbackInfo);
    public delegate void InterstitialVideoShowFailedHandler(string adUnitId, string error, string placement);
    public delegate void InterstitialVideoClickedHandler(string adUnitId, AdCallbackInfo adCallbackInfo);
    public delegate void InterstitialVideoRevenuePaidHandler(string adUnitId, AdCallbackInfo adCallbackInfo);
    public delegate void InterstitialVideoCompletedHandler(string adUnitId, AdCallbackInfo adCallbackInfo);
    public interface IInterstitialVideoPlayer
    {
        event InterstitialVideoDismissedHandler OnDismissed;
        event InterstitialVideoLoadedHandler OnLoaded;
        event InterstitialVideoLoadFailedHandler OnLoadFailed;
        event InterstitialVideoShownHandler OnShown;
        event InterstitialVideoShowFailedHandler OnShowFailed;
        event InterstitialVideoClickedHandler OnClicked;
        event InterstitialVideoRevenuePaidHandler OnRevenuePaid;
        event InterstitialVideoCompletedHandler OnCompleted;
        void RequestInterstitialVideo(string adUnitId);
        bool HasInterstitialVideo(string adUnitId);
        void ShowInterstitialVideo(string adUnitId, string placement);
    }
}
