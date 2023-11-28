#if UNITY_ANDROID
using SspnetSDK.Platform.Android;
#elif UNITY_IPHONE
using SspnetSDK.Platform.iOS;
#else
using SspnetSDK.Platform.Dummy;
#endif
using SspnetSDK.Unfiled;


namespace SspnetSDK.Platform
{
    internal static class ClientFactory
    {
        internal static IAdsClient GetAdsClient()
        {
#if UNITY_ANDROID
			return new AndroidAdsClient();
#elif UNITY_IPHONE
			return IOSAdsClient.Instance;
#else
            return new DummyClient();
#endif
        }
    }
}