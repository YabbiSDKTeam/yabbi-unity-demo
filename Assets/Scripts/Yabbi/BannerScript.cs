using SspnetSDK.Unfiled;
using YabbiSDK.Api;

namespace YabbiSDK.Demo.Scripts
{
    public class BannerScript : UnfiledAdScript, IBannerAdListener
    {
        private void Start()
        {
            Yabbi.SetBannerCallbacks(this);
            var settings = new BannerSettings()
                .SetShowCloseButton(true)
                .SetBannerPosition(BannerPosition.BOTTOM);
            Yabbi.SetBannerCustomSettings(settings);
            InitClickListeners();
        }


        public void OnBannerLoaded(AdPayload adPayload)
        {
            AddLog("OnBannerLoaded");
        }

        public void OnBannerLoadFailed(AdPayload adPayload, AdException error)
        {
            AddLog($"OnBannerLoadFailed: {error.Description}");
        }

        public void OnBannerShown(AdPayload adPayload)
        {
            AddLog("OnBannerShown");
        }

        public void OnBannerShowFailed(AdPayload adPayload, AdException error)
        {
            AddLog($"OnBannerShowFailed: {error.Description}");
        }

        public void OnBannerClosed(AdPayload adPayload)
        {
            AddLog("OnBannerClosed");
        }

        public void OnBannerImpression(AdPayload adPayload)
        {
            AddLog("OnBannerImpression");
        }

        protected override string GetPlacementName()
        {
            return EnvironmentVariables.yabbiBannerUnitID;
        }

        protected override int GetAdType()
        {
            return Yabbi.Banner;
        }
    }
}