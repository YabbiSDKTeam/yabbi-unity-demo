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
        private static IInitializationListener _initializationListener;


        public void Initialize(string publisherID, IInitializationListener listener)
        {
            _initializationListener = listener;
            ObjCBridge.SspnetInitialize(publisherID, OnInitializedCallback);
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
        
        [MonoPInvokeCallback(typeof(UnityBackgroundCallback))]
        internal static void OnInitializedCallback(string description, string message, string caused)
        {
            if (description == null)
            {
                _initializationListener?.OnInitializeSuccess();
            }
            else
            {
                _initializationListener?.OnInitializeFailed(new AdException(description, message, caused));
            }
        }

        #region Intestital Delegate

        [MonoPInvokeCallback(typeof(InterstitialCallbacks))]
        internal static void OnInterstitialLoaded(string placementName)
        {
            _interstitialAdListener?.OnInterstitialLoaded(new AdPayload(placementName));
        }
        
        [MonoPInvokeCallback(typeof(InterstitialFailCallbacks))]
        internal static void OnInterstitialLoadFailed(string placementName, string description, string message, string caused)
        {
            _interstitialAdListener?.OnInterstitialLoadFailed(new AdPayload(placementName),new AdException(description, message, caused));
        }

        [MonoPInvokeCallback(typeof(InterstitialCallbacks))]
        internal static void OnInterstitialShown(string placementName)
        {
            _interstitialAdListener?.OnInterstitialShown(new AdPayload(placementName));
        }
        
        [MonoPInvokeCallback(typeof(InterstitialFailCallbacks))]
        internal static void OnInterstitialShowFailed(string placementName, string description, string message, string caused)
        {
            _interstitialAdListener?.OnInterstitialShowFailed(new AdPayload(placementName),new AdException(description, message, caused));
        }

        [MonoPInvokeCallback(typeof(InterstitialCallbacks))]
        internal static void OnInterstitialClosed(string placementName)
        {
            _interstitialAdListener?.OnInterstitialClosed(new AdPayload(placementName));
        }

        #endregion

        #region Video Delegate

        [MonoPInvokeCallback(typeof(RewardedVideoCallbacks))]
        internal static void OnRewardedLoaded(string placementName)
        {
            _rewardedAdListener?.OnRewardedLoaded(new AdPayload(placementName));
        }
        
        [MonoPInvokeCallback(typeof(RewardedVideoFailCallbacks))]
        internal static void OnRewardedLoadFailed(string placementName, string description, string message, string caused)
        {
            _rewardedAdListener?.OnRewardedLoadFailed(new AdPayload(placementName),new AdException(description, message, caused));
        }

        [MonoPInvokeCallback(typeof(RewardedVideoCallbacks))]
        internal static void OnRewardedShown(string placementName)
        {
            _rewardedAdListener?.OnRewardedShown(new AdPayload(placementName));
        }
        
        [MonoPInvokeCallback(typeof(RewardedVideoFailCallbacks))]
        internal static void OnRewardedShowFailed(string placementName, string description, string message, string caused)
        {
            _rewardedAdListener?.OnRewardedShowFailed(new AdPayload(placementName),new AdException(description, message, caused));
        }

        [MonoPInvokeCallback(typeof(RewardedVideoCallbacks))]
        internal static void OnRewardedClosed(string placementName)
        {
            _rewardedAdListener?.OnRewardedClosed(new AdPayload(placementName));
        }

        [MonoPInvokeCallback(typeof(RewardedVideoCallbacks))]
        internal static void OnRewardedFinished(string placementName)
        {
            _rewardedAdListener?.OnRewardedFinished(new AdPayload(placementName));
        }

        #endregion
    }
}
#endif