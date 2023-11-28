#if UNITY_IPHONE
using AOT;
using SspnetSDK.Unfiled;

namespace SspnetSDK.Platform.iOS
{
    public class IOSAdsClient : IAdsClient
    {
        #region Singleton

        private IOSAdsClient()
        {
        }

        public static IOSAdsClient Instance { get; } = new();

        #endregion

        private static IInterstitialAdListener _interstitialAdListener;
        private static IRewardedAdListener _rewardedAdListener;

        public void Initialize(string publisherID)
        {
            ObjCBridge.SspnetInitialize(publisherID);
        }

        public bool IsInitialized()
        {
            return ObjCBridge.SspnetIsInitialized();
        }

        public bool CanLoadAd(int adType, string placementName)
        {
            return ObjCBridge.SspnetCanLoadAd(adType, placementName);
        }

        public void ShowAd(int adType, string placementName)
        {
            ObjCBridge.SspnetShowAd(adType, placementName);
        }

        public bool IsAdLoaded(int adType, string placementName)
        {
            return ObjCBridge.SspnetIsAdLoaded(adType, placementName);
        }

        public void LoadAd(int adType, string placementName)
        {
            ObjCBridge.SspnetLoadAd(adType, placementName);
        }
        
        public void DestroyAd(int adType)
        {
            ObjCBridge.SspnetDestroyAdByType(adType);
        }

        public void DestroyAd(int adType, string placementName)
        {
            ObjCBridge.SspnetDestroyAd(adType, placementName);
        }

        public void SetInterstitialCallbacks(IInterstitialAdListener adListener)
        {
            _interstitialAdListener = adListener;
            ObjCBridge.SspnetSetInterstitialDelegate(
                OnInterstitialLoaded,
                OnInterstitialLoadFailed,
                OnInterstitialShown,
                OnInterstitialShowFailed,
                OnInterstitialClosed
            );
        }

        public void SetRewardedCallbacks(IRewardedAdListener adListener)
        {
            _rewardedAdListener = adListener;
            ObjCBridge.SspnetSetRewardedDelegate(
                OnRewardedLoaded,
                OnRewardedLoadFailed,
                OnRewardedShown,
                OnRewardedShowFailed,
                OnRewardedClosed,
                OnRewardedFinished
            );
        }

        public void SetCustomParams(string key, string value)
        {
            ObjCBridge.SspnetSetCustomParams(key, value);
        }

        public void SetUserConsent(bool hasConsent)
        {
            ObjCBridge.SspnetSetUserConsent(hasConsent);
        }

        public void EnableDebug(bool enabled)
        {
            ObjCBridge.SspnetEnableDebug(enabled);
        }

        public bool HasUserConsent()
        {
            return ObjCBridge.SspnetHasUserConsent();
        }

        public string GetSdkVersion()
        {
            return ObjCBridge.SspnetGetSdkVersion();
        }

        #region Intestital Delegate

        [MonoPInvokeCallback(typeof(InterstitialCallbacks))]
        internal static void OnInterstitialLoaded()
        {
            _interstitialAdListener?.OnInterstitialLoaded();
        }
        
        [MonoPInvokeCallback(typeof(InterstitialFailCallbacks))]
        internal static void OnInterstitialLoadFailed(string description, string message, string caused)
        {
            _interstitialAdListener?.OnInterstitialLoadFailed(new AdException(description, message, caused));
        }

        [MonoPInvokeCallback(typeof(InterstitialCallbacks))]
        internal static void OnInterstitialShown()
        {
            _interstitialAdListener?.OnInterstitialShown();
        }
        
        [MonoPInvokeCallback(typeof(InterstitialFailCallbacks))]
        internal static void OnInterstitialShowFailed(string description, string message, string caused)
        {
            _interstitialAdListener?.OnInterstitialShowFailed(new AdException(description, message, caused));
        }

        [MonoPInvokeCallback(typeof(InterstitialCallbacks))]
        internal static void OnInterstitialClosed()
        {
            _interstitialAdListener?.OnInterstitialClosed();
        }

        #endregion

        #region Video Delegate

        [MonoPInvokeCallback(typeof(RewardedVideoCallbacks))]
        internal static void OnRewardedLoaded()
        {
            _rewardedAdListener?.OnRewardedLoaded();
        }
        
        [MonoPInvokeCallback(typeof(RewardedVideoFailCallbacks))]
        internal static void OnRewardedLoadFailed(string description, string message, string caused)
        {
            _rewardedAdListener?.OnRewardedLoadFailed(new AdException(description, message, caused));
        }

        [MonoPInvokeCallback(typeof(RewardedVideoCallbacks))]
        internal static void OnRewardedShown()
        {
            _rewardedAdListener?.OnRewardedShown();
        }
        
        [MonoPInvokeCallback(typeof(RewardedVideoFailCallbacks))]
        internal static void OnRewardedShowFailed(string description, string message, string caused)
        {
            _rewardedAdListener?.OnRewardedShowFailed(new AdException(description, message, caused));
        }

        [MonoPInvokeCallback(typeof(RewardedVideoCallbacks))]
        internal static void OnRewardedClosed()
        {
            _rewardedAdListener?.OnRewardedClosed();
        }

        [MonoPInvokeCallback(typeof(RewardedVideoCallbacks))]
        internal static void OnRewardedFinished()
        {
            _rewardedAdListener?.OnRewardedFinished();
        }

        #endregion
    }
}
#endif