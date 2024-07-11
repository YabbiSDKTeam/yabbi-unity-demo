#if UNITY_IPHONE
using AOT;
using SspnetSDK.ConsentManagerSDK.Unfiled;

namespace YabbiSDK.ConsentManagerSDK.Platform.iOS
{
    public class IOSConsentManagerClient : IConsentManagerClient
    {
        private static IConsentListener _listener;

        public void LoadManager()
        {
            ConsentManagerObjCBridge.YabbiLoadConsent();
        }

        public void ShowConsentWindow()
        {
            ConsentManagerObjCBridge.YabbiShowConsent();
        }

        public bool HasConsent()
        {
            return ConsentManagerObjCBridge.YabbiHasConsent();
        }

        public void SetCustomStorage()
        {
            // TODO
        }

        public void EnableDebug(bool isDebug)
        {
            ConsentManagerObjCBridge.YabbiConsentEnableDebug(isDebug);
        }

        public void RegisterCustomVendor(ConsentBuilder builder)
        {
            ConsentManagerObjCBridge.YabbiRegisterCustomConsentVendor(
                builder.GetAppName(),
                builder.GetPolicyURL(),
                builder.GetBundle(),
                builder.GetGdpr()
            );
        }

        public void SetListener(IConsentListener listener)
        {
            _listener = listener;
            ConsentManagerObjCBridge.YabbiSetConsentDelegate(
                OnConsentManagerLoaded,
                OnConsentManagerLoadFailed,
                OnConsentWindowShown,
                OnConsentManagerShownFailed,
                OnConsentWindowClosed
            );
        }

        #region Consent Delegate

        [MonoPInvokeCallback(typeof(ConsentCallbacks))]
        internal static void OnConsentManagerLoaded()
        {
            _listener?.OnConsentManagerLoaded();
        }

        [MonoPInvokeCallback(typeof(ConsentFailCallbacks))]
        internal static void OnConsentManagerLoadFailed(string message)
        {
            _listener?.OnConsentManagerLoadFailed(message);
        }

        [MonoPInvokeCallback(typeof(ConsentCallbacks))]
        internal static void OnConsentWindowShown()
        {
            _listener?.OnConsentWindowShown();
        }

        [MonoPInvokeCallback(typeof(ConsentFailCallbacks))]
        internal static void OnConsentManagerShownFailed(string message)
        {
            _listener?.OnConsentManagerShownFailed(message);
        }

        [MonoPInvokeCallback(typeof(ConsenClosedCallbacks))]
        internal static void OnConsentWindowClosed(bool hasConsent)
        {
            _listener?.OnConsentWindowClosed(hasConsent);
        }

        #endregion
    }
}
#endif