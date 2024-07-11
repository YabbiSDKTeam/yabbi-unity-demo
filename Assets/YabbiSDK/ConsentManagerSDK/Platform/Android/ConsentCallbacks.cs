#if UNITY_ANDROID
using UnityEngine;
using SspnetSDK.ConsentManagerSDK.Unfiled;

namespace YabbiSDK.ConsentManagerSDK.Platform.Android
{
    public class ConsentCallbacks : AndroidJavaProxy
    {
        private readonly IConsentListener _listener;

        internal ConsentCallbacks(IConsentListener listener) : base(ConsentManagerConsants.Listener)
        {
            _listener = listener;
        }

        private void onConsentManagerLoaded() => _listener.OnConsentManagerLoaded();

        private void onConsentManagerLoadFailed(string error) => _listener.OnConsentManagerLoadFailed(error);

        private void onConsentManagerShownFailed(string error) => _listener.OnConsentManagerShownFailed(error);

        private void onConsentWindowShown() => _listener.OnConsentWindowShown();

        public void onConsentWindowClosed(bool hasConsent) => _listener.OnConsentWindowClosed(hasConsent);
    }
}
#endif