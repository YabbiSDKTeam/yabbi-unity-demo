using SspnetSDK.Platform;
using SspnetSDK.Unfiled;

namespace YabbiSDK.Api
{
    public static class Yabbi
    {
        public const int Interstitial = IAdsClient.Interstitial;
        public const int Rewarded = IAdsClient.Rewarded;
        public const int Banner = IAdsClient.Banner;

        private static IAdsClient _client;

        private static IAdsClient GetInstance()
        {
            return _client ??= ClientFactory.GetAdsClient();
        }

        public static void Initialize(string publisherID, IInitializationListener listener)
        {
            GetInstance().SetCustomParams("PUBLISHER_URL", "https://yabbi.me/publishers");
            GetInstance().SetCustomParams("CABINET", "Yabbi");
            GetInstance().SetCustomParams("PLATFORM", "unity");

            GetInstance().Initialize(publisherID, listener);
        }

        public static bool IsInitialized()
        {
            return GetInstance().IsInitialized();
        }

        public static bool CanLoadAd(int adType, string placementName)
        {
            return GetInstance().CanLoadAd(adType, placementName);
        }

        public static void ShowAd(int adType, string placementName)
        {
            GetInstance().ShowAd(adType, placementName);
        }

        public static bool IsAdLoaded(int adType, string placementName)
        {
            return GetInstance().IsAdLoaded(adType, placementName);
        }

        public static void LoadAd(int adType, string placementName)
        {
            GetInstance().LoadAd(adType, placementName);
        }

        public static void DestroyAd(int adType, string placementName)
        {
            GetInstance().DestroyAd(adType, placementName);
        }

        public static void DestroyAd(int adType)
        {
            GetInstance().DestroyAd(adType);
        }

        public static void SetInterstitialCallbacks(IInterstitialAdListener adListener)
        {
            GetInstance().SetInterstitialCallbacks(adListener);
        }

        public static void SetRewardedCallbacks(IRewardedAdListener adListener)
        {
            GetInstance().SetRewardedCallbacks(adListener);
        }
        
        public static void SetBannerCallbacks(IBannerAdListener adListener)
        {
            GetInstance().SetBannerCallbacks(adListener);
        }
        
        public static void SetBannerCustomSettings(BannerSettings settings)
        {
            GetInstance().SetBannerCustomSettings(settings);
        }

        public static void SetCustomParams(string key, string value)
        {
            GetInstance().SetCustomParams(key, value);
        }

        public static void SetUserConsent(bool hasConsent)
        {
            GetInstance().SetUserConsent(hasConsent);
        }

        public static void EnableDebug(bool enabled)
        {
            GetInstance().EnableDebug(enabled);
        }

        public static bool HasUserConsent()
        {
            return GetInstance().HasUserConsent();
        }

        public static string GetSdkVersion()
        {
            return GetInstance().GetSdkVersion();
        }
    }
}