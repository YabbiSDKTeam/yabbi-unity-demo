#if UNITY_ANDROID && !UNITY_EDITOR
using YabbiSDK.ConsentManagerSDK.Platform.Android;
#elif UNITY_IPHONE && !UNITY_EDITOR
using YabbiSDK.ConsentManagerSDK.Platform.iOS;
#else
using YabbiSDK.ConsentManagerSDK.Platform.Dummy;
#endif
using SspnetSDK.ConsentManagerSDK.Unfiled;


namespace YabbiSDK.ConsentManagerSDK.Platform
{
    internal static class ConsentManagerClientFactory
    {
        internal static IConsentManagerClient GetConsentManagerClient()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
			return new AndroidConsentManagerClient();
#elif UNITY_IPHONE && !UNITY_EDITOR
            return new IOSConsentManagerClient();
#else
            return new DummyConsentManagerClient();
#endif
        }
    }
}