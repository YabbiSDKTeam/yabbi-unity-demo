namespace SspnetSDK.Unfiled
{
    public class BannerSettings
    {
        public bool ShowCloseButton { get; private set; }

        public int BannerPosition { get; private set; }

        public int RefreshIntervalSeconds { get; private set; }


        public BannerSettings SetShowCloseButton(bool showCloseButton)
        {
            ShowCloseButton = showCloseButton;
            return this;
        }

        public BannerSettings SetBannerPosition(int bannerPosition)
        {
            BannerPosition = bannerPosition;
            return this;
        }

        public BannerSettings SetRefreshIntervalSeconds(int seconds)
        {
            RefreshIntervalSeconds = seconds;
            return this;
        }
    }
}