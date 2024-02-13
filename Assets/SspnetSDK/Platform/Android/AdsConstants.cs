#if UNITY_ANDROID
namespace SspnetSDK.Platform.Android
{
    internal static class AdsConstants
    {
        public const string UnityActivityClassName = "com.unity3d.player.UnityPlayer";
        public const string InterstitialListener = "sspnet.tech.core.InterstitialListener";
        public const string InitializationListener = "sspnet.tech.core.InitializationListener";
        public const string RewardedListener = "sspnet.tech.core.RewardedListener";
        public const string SspnetCore = "sspnet.tech.core.SspnetCore";
    }
}
#endif