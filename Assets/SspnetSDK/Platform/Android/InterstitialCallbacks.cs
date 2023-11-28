#if UNITY_ANDROID
using SspnetSDK.Unfiled;
using UnityEngine;

namespace SspnetSDK.Platform.Android
{
    public class InterstitialCallbacks : AndroidJavaProxy
    {
        private readonly IInterstitialAdListener _listener;

        internal InterstitialCallbacks(IInterstitialAdListener listener) : base(AdsConstants
            .InterstitialListener)
        {
            _listener = listener;
        }

        private void onInterstitialLoaded() => _listener.OnInterstitialLoaded();

        private void onInterstitialLoadFail(AndroidJavaObject error)
        {
            var description = error.Call<string>("getDescription");
            var message = error.Call<string>("getMessage");
            var caused = error.Call<string>("getCaused");
            _listener.OnInterstitialLoadFailed(new AdException(description, message, caused));
        }

        private void onInterstitialShowFailed(AndroidJavaObject error)
        {
            var description = error.Call<string>("getDescription");
            var message = error.Call<string>("getMessage");
            var caused = error.Call<string>("getCaused");
            _listener.OnInterstitialShowFailed(new AdException(description, message, caused));
        }

        private void onInterstitialShown() => _listener.OnInterstitialShown();

        public void onInterstitialClosed() => _listener.OnInterstitialClosed();
    }
}

#endif