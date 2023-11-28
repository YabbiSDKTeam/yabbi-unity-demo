using YabbiSDK.ConsentManager.Platform;
using SspnetSDK.ConsentManager.Unfiled;

namespace YabbiSDK.ConsentManager.Api
{
    public class ConsentManager
    {
        private IConsentManagerClient _client;

        private IConsentManagerClient GetInstance()
        {
            return _client ??= ConsentManagerClientFactory.GetConsentManagerClient();
        }

        public void LoadManager()
        {
            GetInstance().LoadManager();
        }
        
        public void ShowConsentWindow()
        {
            GetInstance().ShowConsentWindow();
        }
        
        public bool HasConsent()
        {
           return GetInstance().HasConsent();
        }

        public void SetListener(IConsentListener listener)
        {
            GetInstance().SetListener(listener);
        }
        
        public void RegisterCustomVendor(ConsentBuilder builder)
        {
            GetInstance().RegisterCustomVendor(builder);
        }
        
        public void EnableLog(bool isDebug)
        {
            GetInstance().EnableDebug(isDebug);
        }
    }
}