using SspnetSDK.Unfiled;
using YabbiSDK.Api;

namespace YabbiSDK.Demo.Scripts
{
    public class InterstitialScript : UnfiledAdScript, IInterstitialAdListener
    {
        private void Start()
        {
            Yabbi.SetInterstitialCallbacks(this);
            InitClickListeners();
        }

        public void OnInterstitialLoaded(AdPayload adPayload)
        {
            AddLog("OnInterstitialLoaded");
        }

        public void OnInterstitialLoadFailed(AdPayload adPayload, AdException error)
        {
            AddLog($"OnInterstitialLoadFailed: {error.Description}");
        }

        public void OnInterstitialShown(AdPayload adPayload)
        {
            AddLog("OnInterstitialShown");
        }

        public void OnInterstitialShowFailed(AdPayload adPayload, AdException error)
        {
            AddLog($"OnInterstitialShowFailed: {error.Description}");
        }

        public void OnInterstitialClosed(AdPayload adPayload)
        {
            AddLog("OnInterstitialClosed");
        }

        protected override string GetPlacementName()
        {
            return EnvironmentVariables.yabbiInterstitialUnitID;
        }

        protected override int GetAdType()
        {
            return Yabbi.Interstitial;
        }
    }
}