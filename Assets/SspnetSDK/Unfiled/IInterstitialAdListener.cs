namespace SspnetSDK.Unfiled
{
    public interface IInterstitialAdListener
    {
        void OnInterstitialLoaded(AdPayload adPayload);
        void OnInterstitialLoadFailed(AdPayload adPayload, AdException error);
        void OnInterstitialShown(AdPayload adPayload);
        void OnInterstitialShowFailed(AdPayload adPayload, AdException error);
        void OnInterstitialClosed(AdPayload adPayload);
    }
}