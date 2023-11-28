namespace SspnetSDK.ConsentManager.Unfiled
{
    public interface IConsentManagerClient
    {
        public void LoadManager();
        
        public void ShowConsentWindow();

        public bool HasConsent();

        public void SetCustomStorage();

        public void EnableDebug(bool isDebug);

        public void RegisterCustomVendor(ConsentBuilder builder);

        public void SetListener(IConsentListener listener);
    }
}