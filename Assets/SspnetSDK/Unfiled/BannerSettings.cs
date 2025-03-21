namespace SspnetSDK.Unfiled
{
    public class BannerSettings
    {
        public bool ShowCloseButton { get; private set; }

        public BannerSettings SetShowCloseButton(bool showCloseButton)
        {
            ShowCloseButton = showCloseButton;
            return this;
        }
    }
}