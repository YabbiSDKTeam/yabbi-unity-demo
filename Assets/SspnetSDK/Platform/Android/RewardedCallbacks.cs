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

        private void onRewardedLoaded() => _listener.OnRewardedLoaded();

        private void onRewardedLoadFail(AndroidJavaObject error)
        {
            var description = error.Call<string>("getDescription");
            var message = error.Call<string>("getMessage");
            var caused = error.Call<string>("getCaused");
            _listener.OnRewardedLoadFailed(new AdException(description, message, caused));
        }

        private void onRewardedShown() => _listener.OnRewardedShown();

        private void onRewardedShowFailed(AndroidJavaObject error)
        {
            var description = error.Call<string>("getDescription");
            var message = error.Call<string>("getMessage");
            var caused = error.Call<string>("getCaused");
            _listener.OnRewardedShowFailed(new AdException(description, message, caused));
        }

        public void onRewardedClosed() => _listener.OnRewardedClosed();

        public void onRewardedFinished() => _listener.OnRewardedFinished();
    }
}

#endif