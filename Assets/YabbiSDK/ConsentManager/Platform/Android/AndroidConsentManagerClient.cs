#if UNITY_ANDROID
using UnityEngine;
using SspnetSDK.ConsentManager.Unfiled;

namespace YabbiSDK.ConsentManager.Platform.Android
{
    public class AndroidConsentManagerClient : IConsentManagerClient
    {
        
        private AndroidJavaObject _activity;
        
        private AndroidJavaObject _instance;
        private AndroidJavaObject GetActivity()
        {
            if (_activity != null) return _activity;
            var playerClass = new AndroidJavaClass(ConsentManagerConsants.UnityActivityClassName);
            _activity = playerClass.GetStatic<AndroidJavaObject>("currentActivity");

            return _activity;
        }

        private AndroidJavaObject GetInstance()
        {
            return _instance ??= new AndroidJavaObject(ConsentManagerConsants.ConsentManager);
        }
        
        public void LoadManager()
        {
            GetInstance().Call("loadManager");
        }

        public void ShowConsentWindow()
        {
            GetInstance().Call("showConsentWindow", GetActivity());
        }

        public bool HasConsent()
        {
            return GetInstance().Call<bool>("hasConsent", GetActivity());
        }

        public void SetCustomStorage()
        {
            // TODO
        }

        public void EnableDebug(bool isDebug)
        {
            GetInstance().Call("enableDebug", isDebug);
        }

        public void RegisterCustomVendor(ConsentBuilder builder)
        {
            var nativeBuilder = new AndroidJavaObject(ConsentManagerConsants.Builder)
                .Call<AndroidJavaObject>("appendPolicyURL", builder.GetPolicyURL())
                .Call<AndroidJavaObject>("appendBundle", builder.GetBundle())
                .Call<AndroidJavaObject>("appendName", builder.GetAppName())
                .Call<AndroidJavaObject>("appendGDPR", builder.GetGdpr());
            
            GetInstance().Call("registerCustomVendor", nativeBuilder);
        }

        public void SetListener(IConsentListener listener)
        {
            GetInstance().Call("setListener", new ConsentCallbacks(listener));
        }
    }
}
#endif