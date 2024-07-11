using SspnetSDK.Unfiled;
using UnityEngine;

namespace SspnetSDK.Platform.Dummy
{
    public class DummyClient : IAdsClient
    {
        #region Sspnet

        public void Initialize(string publisherID, IInitializationListener listener)
        {
            DebugLog("Initialize");
        }

        public bool IsInitialized()
        {
            DebugLog("IsInitialized");
            return false;
        }

        public bool CanLoadAd(int adType)
        {
            DebugLog("CanLoadAd");
            return false;
        }

        public bool CanLoadAd(int adType, string placementName)
        {
            DebugLog("CanLoadAd");
            return false;
        }

        public void ShowAd(int adType)
        {
            DebugLog("ShowAd");
        }

        public void ShowAd(int adType, string placementName)
        {
            DebugLog("ShowAd");
        }

        public bool IsAdLoaded(int adType)
        {
            DebugLog("isLoaded");
            return false;
        }

        public bool IsAdLoaded(int adType, string placementName)
        {
            DebugLog("isLoaded");
            return false;
        }

        public void LoadAd(int adType, string placementName)
        {
            DebugLog("LoadAd");
        }

        public void DestroyAd(int adType)
        {
            DebugLog("DestroyAd");
        }

        public void DestroyAd(int adType, string placementName)
        {
            DebugLog("DestroyAd");
        }

        public void SetInterstitialCallbacks(IInterstitialAdListener adListener)
        {
            DebugLog("SetInterstitialCallbacks");
        }

        public void SetRewardedCallbacks(IRewardedAdListener adListener)
        {
            DebugLog("SetRewardedCallbacks");
        }

        public void SetCustomParams(string key, string value)
        {
            DebugLog("setCustomParams");
        }

        public void SetUserConsent(bool hasConsent)
        {
            DebugLog("SetUserConsent");
        }

        public void EnableDebug(bool enabled)
        {
            DebugLog("EnableDebug");
        }

        public bool HasUserConsent()
        {
            DebugLog("HasUserConsent");
            return false;
        }

        public string GetSdkVersion()
        {
            DebugLog("HasUserConsent");
            return "";
        }

        #endregion

        #region Debug

        private static void DebugLog(string method)
        {
            Debug.Log(
                $"Call to {method} on not supported platform. To test advertising, install your application on the Android/iOS device.");
        }

        #endregion
    }
}