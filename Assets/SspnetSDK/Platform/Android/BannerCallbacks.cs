#if UNITY_ANDROID
using SspnetSDK.Unfiled;
using UnityEngine;

namespace SspnetSDK.Platform.Android
{
    public class BannerCallbacks : AndroidJavaProxy
    {
        private readonly IBannerAdListener _listener;

        internal BannerCallbacks(IBannerAdListener listener) : base(AndroidConstants.BannerListener)
        {
            _listener = listener;
        }

        private void onBannerLoaded(AndroidJavaObject adPayload)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            _listener.OnBannerLoaded(new AdPayload(placementName));
        }

        private void onBannerLoadFailed(AndroidJavaObject adPayload, AndroidJavaObject error)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            var description = error.Call<string>("getDescription");
            var message = error.Call<string>("getMessage");
            var caused = error.Call<string>("getCaused");
            _listener.OnBannerLoadFailed(new AdPayload(placementName),new AdException(description, message, caused));
        }

        private void onBannerShown(AndroidJavaObject adPayload)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            _listener.OnBannerShown(new AdPayload(placementName));
        }

        private void onBannerShowFailed(AndroidJavaObject adPayload, AndroidJavaObject error)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            var description = error.Call<string>("getDescription");
            var message = error.Call<string>("getMessage");
            var caused = error.Call<string>("getCaused");
            _listener.OnBannerShowFailed(new AdPayload(placementName),new AdException(description, message, caused));
        }

        public void onBannerClosed(AndroidJavaObject adPayload)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            _listener.OnBannerClosed(new AdPayload(placementName));
        }
        
        public void onBannerImpression(AndroidJavaObject adPayload)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            _listener.OnBannerImpression(new AdPayload(placementName));
        }
    }
}

#endif