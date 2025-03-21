#if UNITY_ANDROID && !UNITY_EDITOR
using SspnetSDK.Platform.Android;
#elif UNITY_IPHONE && !UNITY_EDITOR
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
#if UNITY_ANDROID && !UNITY_EDITOR
			    return new AndroidAdsClient();
#elif UNITY_IPHONE && !UNITY_EDITOR
            return IOSAdsClient.Instance;
#else
            return new DummyClient();
#endif
        }
    }
}