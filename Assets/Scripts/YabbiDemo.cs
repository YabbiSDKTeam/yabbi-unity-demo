using Scripts.Utils;
using SspnetSDK.Unfiled;
using UnityEngine;
using YabbiSDK.Api;

namespace YabbiSDK.Demo.Scripts
{
    public class YabbiDemo : MonoBehaviour, IInitializationListener
    {
        private void Start()
        {
            Yabbi.EnableDebug(true);
            Yabbi.Initialize(EnvironmentVariables.publisherID, this);
        }

        public void OnInitializeSuccess()
        {
        }

        public void OnInitializeFailed(AdException error)
        {
        }

        public void ShowInterstitialScence()
        {
            SceneNavigationManager.Instance.LoadSceneAdditively("InterstitialScence");
        }

        public void ShowRewardedScence()
        {
            SceneNavigationManager.Instance.LoadSceneAdditively("RewardedScence");
        }

        public void ShowBannerScence()
        {
            SceneNavigationManager.Instance.LoadSceneAdditively("BannerScence");
        }

        public void ShowConsentScence()
        {
            SceneNavigationManager.Instance.LoadSceneAdditively("ConsentManagerScence");
        }
    }
}