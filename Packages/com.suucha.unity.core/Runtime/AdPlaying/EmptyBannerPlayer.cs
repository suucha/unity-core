namespace SuuchaStudio.Unity.Core.AdPlaying
{
#pragma warning disable CS0067
    public class EmptyBannerPlayer : IBannerPlayer
    {
        public event BannerLoadedHandler OnLoaded;
        public event BannerFailedHandler OnLoadFailed;
        public event BannerClickedHandler OnClicked;
        public event BannerRevenuePaidHandler OnRevenuePaid;
        private static IBannerPlayer instance;
        public static IBannerPlayer Instance
        {
            get
            {
                instance ??= new EmptyBannerPlayer();
                return instance;
            }
        }
        public void HideBanner(string adUnitId)
        {

        }

        public void RequestBanner(string adUnitId, BannerPosition position)
        {

        }

        public void ShowBanner(string adUnitId)
        {

        }
    }
}
