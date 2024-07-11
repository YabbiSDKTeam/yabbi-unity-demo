using UnityEngine;
using SspnetSDK.ConsentManagerSDK.Unfiled;

namespace YabbiSDK.ConsentManagerSDK.Platform.Dummy
{
    public class DummyConsentManagerClient : IConsentManagerClient
    {
        public void LoadManager()
        {
            DebugLog("LoadManager");
        }

        public void ShowConsentWindow()
        {
            DebugLog("ShowConsentWindow");
        }

        public bool HasConsent()
        {
            DebugLog("HasConsent");
            return false;
        }

        public void SetCustomStorage()
        {
            DebugLog("SetCustomStorage");
        }

        public void EnableDebug(bool isDebug)
        {
            DebugLog("EnableDebug");
        }

        public void RegisterCustomVendor(ConsentBuilder builder)
        {
            DebugLog("RegisterCustomVendor");
        }

        public void SetListener(IConsentListener listener)
        {
            DebugLog("SetListener");
        }

        private static void DebugLog(string method)
        {
            Debug.Log(
                $"Call to {method} on not supported platform. To test advertising, install your application on the Android/iOS device.");
        }
    }
}