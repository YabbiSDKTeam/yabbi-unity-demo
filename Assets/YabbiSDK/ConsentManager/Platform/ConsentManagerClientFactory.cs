using SspnetSDK.ConsentManagerSDK.Unfiled;

#if UNITY_ANDROID
using YabbiSDK.ConsentManagerSDK.Platform.Android;
#elif UNITY_IPHONE
using YabbiSDK.ConsentManagerSDK.Platform.iOS;
#else
using YabbiSDK.ConsentManagerSDK.Platform.Dummy;
#endif


namespace YabbiSDK.ConsentManagerSDK.Platform
{
    internal static class ConsentManagerClientFactory
    {
        internal static IConsentManagerClient GetConsentManagerClient()
        {
#if UNITY_ANDROID
			return new AndroidConsentManagerClient();
#elif UNITY_IPHONE
			return new IOSConsentManagerClient();
#else
            return new DummyConsentManagerClient();
#endif
        }
    }
}