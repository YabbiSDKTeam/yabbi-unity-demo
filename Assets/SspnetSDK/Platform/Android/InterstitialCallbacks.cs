#if UNITY_ANDROID
using SspnetSDK.Unfiled;
using UnityEngine;

namespace SspnetSDK.Platform.Android
{
    public class InterstitialCallbacks : AndroidJavaProxy
    {
        private readonly IInterstitialAdListener _listener;

        internal InterstitialCallbacks(IInterstitialAdListener listener) : base(AndroidConstants
            .InterstitialListener)
        {
            _listener = listener;
        }

        private void onInterstitialLoaded(AndroidJavaObject adPayload)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            _listener.OnInterstitialLoaded(new AdPayload(placementName));
        }

        private void onInterstitialLoadFail(AndroidJavaObject adPayload, AndroidJavaObject error)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            var description = error.Call<string>("getDescription");
            var message = error.Call<string>("getMessage");
            var caused = error.Call<string>("getCaused");
            _listener.OnInterstitialLoadFailed(new AdPayload(placementName),new AdException(description, message, caused));
        }

        private void onInterstitialShowFailed(AndroidJavaObject adPayload, AndroidJavaObject error)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            var description = error.Call<string>("getDescription");
            var message = error.Call<string>("getMessage");
            var caused = error.Call<string>("getCaused");
            _listener.OnInterstitialShowFailed(new AdPayload(placementName),new AdException(description, message, caused));
        }

        private void onInterstitialShown(AndroidJavaObject adPayload)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            _listener.OnInterstitialShown(new AdPayload(placementName));
        }

        public void onInterstitialClosed(AndroidJavaObject adPayload)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            _listener.OnInterstitialClosed(new AdPayload(placementName));
        }
    }
}

#endif