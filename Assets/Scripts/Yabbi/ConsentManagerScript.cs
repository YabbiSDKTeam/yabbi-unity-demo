using SspnetSDK.ConsentManagerSDK.Unfiled;
using UnityEngine;
using UnityEngine.UI;
using YabbiSDK.Api;
using YabbiSDK.ConsentManagerSDK.Api;

namespace YabbiSDK.Demo.Scripts
{
    public class ConsentManagerScript : MonoBehaviour, IConsentListener
    {
        public Button loadGdprButton;
        public Button loadNoGdprButton;
        public Text logger;
        private readonly ConsentManager manager = new();

        private void Start()
        {
            manager.SetListener(this);
            manager.EnableLog(true);

            LoadWindow(true);

            loadGdprButton.onClick.AddListener(LoadGdprButtonClick);
            loadNoGdprButton.onClick.AddListener(LoadNoGdprButtonClick);
        }

        public void OnConsentManagerLoaded()
        {
            AddLog("OnConsentManagerLoaded");
            manager.ShowConsentWindow();
        }

        public void OnConsentManagerLoadFailed(string error)
        {
            AddLog($"OnConsentManagerLoadFailed: {error}");
        }

        public void OnConsentWindowShown()
        {
            AddLog("onConsentWindowShown");
        }

        public void OnConsentManagerShownFailed(string error)
        {
            AddLog($"OnConsentManagerShownFailed: {error}");
        }

        public void OnConsentWindowClosed(bool hasConsent)
        {
            AddLog($"OnConsentWindowClosed: {hasConsent}");
            Yabbi.SetUserConsent(hasConsent);
        }

        private void LoadGdprButtonClick()
        {
            LoadWindow(true);
        }

        private void LoadNoGdprButtonClick()
        {
            LoadWindow(false);
        }

        private void LoadWindow(bool isGdpr)
        {
            var builder = new ConsentBuilder()
                .AppendPolicyURL("https://yabbi.me/privacy-policies")
                .AppendGdpr(isGdpr);

            manager.RegisterCustomVendor(builder);
            manager.LoadManager();
        }

        private void AddLog(string message)
        {
            var current = logger.text;
            logger.text = $"{current}\n{message}";
        }
    }
}