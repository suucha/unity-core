namespace SuuchaStudio.Unity.Core.AdPlaying
{
    public delegate void BannerLoadedHandler(string adUnitId, AdCallbackInfo adCallbackInfo);
    public delegate void BannerFailedHandler(string adUnitId, string errorMessage);
    public delegate void BannerClickedHandler(string adUnitId, AdCallbackInfo adCallbackInfo);
    public delegate void BannerRevenuePaidHandler(string adUnitId, AdCallbackInfo adCallbackInfo);

    public interface IBannerPlayer
    {
        event BannerLoadedHandler OnLoaded;
        event BannerFailedHandler OnLoadFailed;
        event BannerClickedHandler OnClicked;
        event BannerRevenuePaidHandler OnRevenuePaid;
        void RequestBanner(string adUnitId, BannerPosition position);
        void ShowBanner(string adUnitId);
        void HideBanner(string adUnitId);
    }
}
