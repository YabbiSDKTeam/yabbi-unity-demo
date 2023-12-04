using System;
using SspnetSDK.ConsentManagerSDK.Unfiled;
using UnityEngine;
using UnityEngine.UI;
using SspnetSDK.Unfiled;
using YabbiSDK.Api;
using YabbiSDK.ConsentManagerSDK.Api;

namespace YabbiSDK.Demo.Scripts
{
    public class YabbiDemo : MonoBehaviour, IInterstitialAdListener, IRewardedAdListener, IConsentListener
    {
        public Text logger;
        private readonly ConsentManager _consentManager = new();

        private void Start()
        {
            InitializeSDK();
        }

        public void LoadInterstitialAd()
        {
            try
            {
                Yabbi.LoadAd(Yabbi.Interstitial, "b8359c60-9bde-47c9-85ff-3c7afd2bd982");
            }
            catch (Exception error)
            {
                LogEvent($"{error}");
            }
           
        }

        public void ShowInterstitialAd()
        {
            Yabbi.ShowAd(Yabbi.Interstitial, "b8359c60-9bde-47c9-85ff-3c7afd2bd982");
        }

        public void DestroyInterstitialAd()
        {
            Yabbi.DestroyAd(Yabbi.Interstitial);
        }

        public void LoadRewardedAd()
        {
            Yabbi.LoadAd(Yabbi.Rewarded, "eaac7a7f-b0b0-46d2-ac95-bd58578e9e29");
        }

        public void ShowRewardedAd()
        {
            Yabbi.ShowAd(Yabbi.Rewarded, "eaac7a7f-b0b0-46d2-ac95-bd58578e9e29");
        }

        public void DestroyRewardedAd()
        {
            Yabbi.DestroyAd(Yabbi.Rewarded);
        }

        public void ShowConsent()
        {
            _consentManager.ShowConsentWindow();
        }

        private void InitializeSDK()
        {
            try
            {
                Yabbi.SetInterstitialCallbacks(this);
                Yabbi.SetRewardedCallbacks(this);

                Yabbi.SetUserConsent(true);
                Yabbi.Initialize("65057899-a16a-4877-989b-38c432a7fa15");
                
                _consentManager.SetListener(this);
                
                var builder = new ConsentBuilder()
                    .AppendPolicyURL("https://yabbi.me/privacy-policies")
                    .AppendGdpr(true)
                    .AppendBundle("me.yabbi.ads.app")
                    .AppendName("Example name");
        
                _consentManager.RegisterCustomVendor(builder);
                _consentManager.LoadManager();
                _consentManager.EnableLog(true);
            }
            catch (Exception error)
            {
                LogEvent($"{error}");
            }
        }

        private void LogEvent(string message)
        {
            var current = logger.text;
            logger.text = $"{current}\n{message}";
        }

        public void OnInterstitialLoaded()
        {
            LogEvent("OnInterstitialLoaded");
        }

        public void OnInterstitialLoadFailed(AdException error)
        {
            LogEvent($"OnInterstitialLoadFailed: {error.Description}");
        }

        public void OnInterstitialShown()
        {
            LogEvent("OnInterstitialShown");
        }

        public void OnInterstitialShowFailed(AdException error)
        {
            LogEvent($"OnInterstitialShowFailed: {error.Description}");
        }

        public void OnInterstitialClosed()
        {
            LogEvent("OnInterstitialClosed");
        }

        public void OnRewardedLoaded()
        {
            LogEvent("OnRewardedLoaded");
        }

        public void OnRewardedLoadFailed(AdException error)
        {
            LogEvent($"OnRewardedLoadFailed: {error.Description}");
        }

        public void OnRewardedShowFailed(AdException error)
        {
            LogEvent($"OnRewardedShowFailed: {error.Description}");
        }

        public void OnRewardedShown()
        {
            LogEvent("OnRewardedShown");
        }

        public void OnRewardedFinished()
        {
            LogEvent("OnRewardedFinished");
        }

        public void OnRewardedClosed()
        {
            LogEvent("OnRewardedClosed");
        }

        public void OnConsentManagerLoaded()
        {
            LogEvent("onConsentManagerLoaded");
        }

        public void OnConsentManagerLoadFailed(string error)
        {
            LogEvent($"onConsentManagerLoadFailed - {error}");
        }

        public void OnConsentWindowShown()
        {
            LogEvent("onConsentWindowShown");
        }

        public void OnConsentManagerShownFailed(string error)
        {
            LogEvent($"onConsentManagerShownFailed - ${error}");
        }

        public void OnConsentWindowClosed(bool hasConsent)
        {
            Yabbi.SetUserConsent(hasConsent);
            LogEvent($"onConsentWindowClosed - {hasConsent}");
        }
    }
}