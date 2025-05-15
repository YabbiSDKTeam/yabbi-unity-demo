#if UNITY_ANDROID
using SspnetSDK.Unfiled;
using UnityEngine;

namespace SspnetSDK.Platform.Android
{
    public class AndroidAdsClient : IAdsClient
    {
        private AndroidJavaObject _activity;


        private AndroidJavaClass _nativeClass;

        public void Initialize(string publisherID, IInitializationListener listener)
        {
            GetCoreClass().CallStatic("initialize", publisherID, new InitializationCallbacks(listener));
        }

        public bool IsInitialized()
        {
            return GetCoreClass().CallStatic<bool>("isInitialized");
        }

        public bool CanLoadAd(int adType, string placementName)
        {
            return GetCoreClass().CallStatic<bool>("canLoadAd", adType, placementName);
        }

        public void ShowAd(int adType, string placementName)
        {
            GetCoreClass().CallStatic("showAd", GetActivity(), adType, placementName);
        }

        public bool IsAdLoaded(int adType, string placementName)
        {
            return GetCoreClass().CallStatic<bool>("isAdLoaded", adType, placementName);
        }

        public void LoadAd(int adType, string placementName)
        {
            GetCoreClass().CallStatic("loadAd", GetActivity(), adType, placementName);
        }

        public void DestroyAd(int adType, string placementName)
        {
            GetCoreClass().CallStatic("destroyAd", adType, placementName);
        }

        public void DestroyAd(int adType)
        {
            GetCoreClass().CallStatic("destroyAd", adType);
        }

        public void SetInterstitialCallbacks(IInterstitialAdListener adListener)
        {
            GetCoreClass().CallStatic("setInterstitialListener", new InterstitialCallbacks(adListener));
        }

        public void SetRewardedCallbacks(IRewardedAdListener adListener)
        {
            GetCoreClass().CallStatic("setRewardedListener", new RewardedCallbacks(adListener));
        }

        public void SetBannerCallbacks(IBannerAdListener adListener)
        {
            GetCoreClass().CallStatic("setBannerListener", new BannerCallbacks(adListener));
        }

        public void SetBannerCustomSettings(BannerSettings settings)
        {
            using var bannerSettingsClass = new AndroidJavaClass(AndroidConstants.BannerSettings);
            // Получаем Builder через статический метод builder()
            var builder = bannerSettingsClass.CallStatic<AndroidJavaObject>("builder");

            // Устанавливаем необходимые параметры
            builder.Call<AndroidJavaObject>("setShowCloseButton", settings.ShowCloseButton);
            builder.Call<AndroidJavaObject>("setBannerPosition", settings.BannerPosition);
            builder.Call<AndroidJavaObject>("setRefreshIntervalSeconds", settings.RefreshIntervalSeconds);

            // Создаем нативный объект BannerSettings через build()
            var nativeBannerSettings = builder.Call<AndroidJavaObject>("build");

            // Передаем созданный объект в основной класс
            GetCoreClass().CallStatic("setBannerCustomSettings", nativeBannerSettings);
        }


        public void SetCustomParams(string key, string value)
        {
            GetCoreClass().CallStatic("setCustomParams", key, value);
        }

        public void SetUserConsent(bool hasConsent)
        {
            GetCoreClass().CallStatic("setUserConsent", hasConsent);
        }

        public void EnableDebug(bool enabled)
        {
            GetCoreClass().CallStatic("enableDebug", enabled);
        }

        public bool HasUserConsent()
        {
            return GetCoreClass().CallStatic<bool>("hasUserConsent");
        }

        public string GetSdkVersion()
        {
            return GetCoreClass().CallStatic<string>("getSdkVersion");
        }

        private AndroidJavaClass GetCoreClass()
        {
            return _nativeClass ??= new AndroidJavaClass(AndroidConstants.SspnetCore);
        }

        private AndroidJavaObject GetActivity()
        {
            if (_activity != null) return _activity;
            var playerClass = new AndroidJavaClass(AndroidConstants.UnityActivityClassName);
            _activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");

            return _activity;
        }

        private static AndroidJavaObject BoolToAndroid(bool value)
        {
            var boleanClass = new AndroidJavaClass("java.lang.Boolean");
            var boolean = boleanClass.CallStatic<AndroidJavaObject>("valueOf", value);
            return boolean;
        }
    }
}
#endif