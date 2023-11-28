#if UNITY_ANDROID
using SspnetSDK.Unfiled;
using UnityEngine;

namespace SspnetSDK.Platform.Android
{
    public class AndroidAdsClient : IAdsClient
    {
        private AndroidJavaObject _activity;


        private AndroidJavaClass _nativeClass;

        private AndroidJavaClass GetCoreClass()
        {
            return _nativeClass ??= new AndroidJavaClass(AdsConstants.SspnetCore);
        }
        
        public void Initialize(string publisherID)
        {
            GetCoreClass().CallStatic("initialize", publisherID);
        }

        public bool IsInitialized()
        {
           return GetCoreClass().CallStatic<bool>("isInitialized");
        }

        private AndroidJavaObject GetActivity()
        {
            if (_activity != null) return _activity;
            var playerClass = new AndroidJavaClass(AdsConstants.UnityActivityClassName);
            _activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");

            return _activity;
        }

        private static AndroidJavaObject BoolToAndroid(bool value)
        {
            var boleanClass = new AndroidJavaClass("java.lang.Boolean");
            var boolean = boleanClass.CallStatic<AndroidJavaObject>("valueOf", value);
            return boolean;
        }

        public bool CanLoadAd(int adType, string placementName)
        {
            return GetCoreClass().CallStatic<bool>("canLoadAd", adType, placementName);
        }

        public void ShowAd(int adType, string placementName)
        {
            GetCoreClass().CallStatic("showAd",GetActivity(), adType, placementName);
        }

        public bool IsAdLoaded(int adType, string placementName)
        {
            return GetCoreClass().CallStatic<bool>("isAdLoaded", adType, placementName);
        }

        public void LoadAd(int adType, string placementName)
        {
            GetCoreClass().CallStatic("loadAd",GetActivity(), adType, placementName);
        }

        public void DestroyAd(int adType, string placementName)
        {
            GetCoreClass().CallStatic("destroyAd", placementName);
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
    }
}
#endif