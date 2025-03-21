using UnityEngine;
using UnityEngine.UI;
using YabbiSDK.Api;

namespace YabbiSDK.Demo.Scripts
{
    public abstract class UnfiledAdScript : MonoBehaviour
    {
        public Button loadAdButton;
        public Button showAdButton;
        public Button destroyAdButton;
        public Text logger;


        protected void InitClickListeners()
        {
            loadAdButton.onClick.AddListener(LoadButtonClick);
            showAdButton.onClick.AddListener(ShowButtonClick);
            destroyAdButton.onClick.AddListener(DestroyButtonClick);
        }

        private void LoadButtonClick()
        {
            if (Yabbi.CanLoadAd(GetAdType(), GetPlacementName()))
            {
                AddLog("Ad start to load.");
                Yabbi.LoadAd(GetAdType(), GetPlacementName());
            }
            else
            {
                AddLog("SDK can't start load ad.");
            }
        }

        private void ShowButtonClick()
        {
            if (Yabbi.IsAdLoaded(GetAdType(), GetPlacementName()))
                Yabbi.ShowAd(GetAdType(), GetPlacementName());
            else
                AddLog("Ad is not loaded yet");
        }

        private void DestroyButtonClick()
        {
            Yabbi.DestroyAd(GetAdType(), GetPlacementName());
            AddLog("Ad was destroyed.");
        }

        protected abstract string GetPlacementName();

        protected abstract int GetAdType();

        protected void AddLog(string message)
        {
            var current = logger.text;
            logger.text = $"{current}\n{message}";
        }
    }
}