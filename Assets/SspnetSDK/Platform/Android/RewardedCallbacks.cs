#if UNITY_ANDROID
using SspnetSDK.Unfiled;
using UnityEngine;

namespace SspnetSDK.Platform.Android
{
    public class RewardedCallbacks : AndroidJavaProxy
    {
        private readonly IRewardedAdListener _listener;

        internal RewardedCallbacks(IRewardedAdListener listener) : base(AdsConstants.RewardedListener)
        {
            _listener = listener;
        }

        private void onRewardedLoaded(AndroidJavaObject adPayload)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            _listener.OnRewardedLoaded(new AdPayload(placementName));
        }

        private void onRewardedLoadFail(AndroidJavaObject adPayload, AndroidJavaObject error)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            var description = error.Call<string>("getDescription");
            var message = error.Call<string>("getMessage");
            var caused = error.Call<string>("getCaused");
            _listener.OnRewardedLoadFailed(new AdPayload(placementName),new AdException(description, message, caused));
        }

        private void onRewardedShown(AndroidJavaObject adPayload)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            _listener.OnRewardedShown(new AdPayload(placementName));
        }

        private void onRewardedShowFailed(AndroidJavaObject adPayload, AndroidJavaObject error)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            var description = error.Call<string>("getDescription");
            var message = error.Call<string>("getMessage");
            var caused = error.Call<string>("getCaused");
            _listener.OnRewardedShowFailed(new AdPayload(placementName),new AdException(description, message, caused));
        }

        public void onRewardedClosed(AndroidJavaObject adPayload)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            _listener.OnRewardedClosed(new AdPayload(placementName));
        }
        
        public void onRewardedVideoStarted(AndroidJavaObject adPayload)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            _listener.OnRewardedVideoStarted(new AdPayload(placementName));
        }
        
        public void onRewardedVideoCompleted(AndroidJavaObject adPayload)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            _listener.OnRewardedVideoCompleted(new AdPayload(placementName));
        }

        public void onUserRewarded(AndroidJavaObject adPayload)
        {
            var placementName = adPayload.Call<string>("getPlacementName");
            _listener.OnUserRewarded(new AdPayload(placementName));
        }
    }
}

#endif