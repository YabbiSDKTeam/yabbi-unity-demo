namespace SspnetSDK.Unfiled
{
    public interface IInterstitialAdListener
    {
        void OnInterstitialLoaded();
        void OnInterstitialLoadFailed(AdException error);
        void OnInterstitialShown();
        void OnInterstitialShowFailed(AdException error);
        void OnInterstitialClosed();
    }
}