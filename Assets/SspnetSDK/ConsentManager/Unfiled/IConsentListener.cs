namespace SspnetSDK.ConsentManager.Unfiled
{
    public interface IConsentListener
    {
        public void OnConsentManagerLoaded();
        public void OnConsentManagerLoadFailed(string error);
        public void OnConsentWindowShown();
        public void OnConsentManagerShownFailed(string error);
        public void OnConsentWindowClosed(bool hasConsent);
    }
}