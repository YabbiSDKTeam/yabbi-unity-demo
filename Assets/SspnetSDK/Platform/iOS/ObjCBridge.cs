#if UNITY_IPHONE
using System.Runtime.InteropServices;

namespace SspnetSDK.Platform.iOS
{
    internal delegate void InterstitialCallbacks(string placementName);

    internal delegate void InterstitialFailCallbacks(string placementName, string description, string message,
        string caused);

    internal delegate void RewardedVideoCallbacks(string placementName);

    internal delegate void RewardedVideoFailCallbacks(string placementName, string description, string message,
        string caused);
    
    internal delegate void BannerCallbacks(string placementName);

    internal delegate void BannerFailCallbacks(string placementName, string description, string message,
        string caused);

    internal delegate void UnityBackgroundCallback(string description, string message, string caused);

    internal static class ObjCBridge
    {
        #region Declare external C interface

        [DllImport("__Internal")]
        internal static extern void SspnetInitialize(string publisherID, UnityBackgroundCallback backgroundCallback);

        [DllImport("__Internal")]
        internal static extern bool SspnetIsInitialized();

        [DllImport("__Internal")]
        internal static extern void SspnetLoadAd(int adType, string placementName);

        [DllImport("__Internal")]
        internal static extern bool SspnetCanLoadAd(int adType, string placementName);

        [DllImport("__Internal")]
        internal static extern bool SspnetIsAdLoaded(int adType, string placementName);

        [DllImport("__Internal")]
        internal static extern void SspnetShowAd(int adType, string placementName);

        [DllImport("__Internal")]
        internal static extern void SspnetDestroyAd(int adType, string placementName);

        [DllImport("__Internal")]
        internal static extern void SspnetDestroyAdByType(int adType);

        [DllImport("__Internal")]
        internal static extern void SspnetSetCustomParams(string key, string value);

        [DllImport("__Internal")]
        internal static extern void SspnetSetUserConsent(bool hasConsent);

        [DllImport("__Internal")]
        internal static extern void SspnetEnableDebug(bool enabled);

        [DllImport("__Internal")]
        internal static extern bool SspnetHasUserConsent();

        [DllImport("__Internal")]
        internal static extern string SspnetGetSdkVersion();

        [DllImport("__Internal")]
        internal static extern void SspnetSetInterstitialDelegate(
            InterstitialCallbacks onLoaded,
            InterstitialFailCallbacks onLoadedFailed,
            InterstitialCallbacks onShown,
            InterstitialFailCallbacks onShownFailed,
            InterstitialCallbacks onClosed
        );

        [DllImport("__Internal")]
        internal static extern void SspnetSetRewardedDelegate(
            RewardedVideoCallbacks onLoaded,
            RewardedVideoFailCallbacks onLoadedFailed,
            RewardedVideoCallbacks onShown,
            RewardedVideoFailCallbacks onShownFailed,
            RewardedVideoCallbacks onClosed,
            RewardedVideoCallbacks onVideoStarted,
            RewardedVideoCallbacks onVideoCompleted,
            RewardedVideoCallbacks onUserRewarded
        );

        [DllImport("__Internal")]
        internal static extern void SspnetSetBannerDelegate(
            BannerCallbacks onLoaded,
            BannerFailCallbacks onLoadedFailed,
            BannerCallbacks onShown,
            BannerFailCallbacks onShownFailed,
            BannerCallbacks onClosed,
            BannerCallbacks onImpression
        );
        
        [DllImport("__Internal")]
        internal static extern void SspneSetCustomBannerSettings(bool showCloseButton);

        #endregion
    }
}
#endif