#if UNITY_IPHONE
using System.Runtime.InteropServices;

namespace YabbiSDK.ConsentManagerSDK.Platform.iOS
{
    internal delegate void ConsentCallbacks();

    internal delegate void ConsentFailCallbacks(string messgae);

    internal delegate void ConsenClosedCallbacks(bool hasConsent);

    internal static class ConsentManagerObjCBridge
    {
        #region Declare external C interface

        [DllImport("__Internal")]
        internal static extern void YabbiLoadConsent();

        [DllImport("__Internal")]
        internal static extern void YabbiShowConsent();

        [DllImport("__Internal")]
        internal static extern bool YabbiHasConsent();

        [DllImport("__Internal")]
        internal static extern void YabbiConsentEnableDebug(bool isDebug);

        [DllImport("__Internal")]
        internal static extern void YabbiRegisterCustomConsentVendor(
            string appName,
            string policyUrl,
            string bundle,
            bool isGdpr);

        [DllImport("__Internal")]
        internal static extern void YabbiSetConsentDelegate(
            ConsentCallbacks onLoaded,
            ConsentFailCallbacks onLoadedFailed,
            ConsentCallbacks onShown,
            ConsentFailCallbacks onShownFailed,
            ConsenClosedCallbacks onClosed
        );

        #endregion
    }
}
#endif