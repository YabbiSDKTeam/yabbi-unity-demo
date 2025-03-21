namespace SspnetSDK.Unfiled
{
    public interface IBannerAdListener
    {
        void OnBannerLoaded(AdPayload adPayload);
        void OnBannerLoadFailed(AdPayload adPayload, AdException error);
        void OnBannerShown(AdPayload adPayload);
        void OnBannerShowFailed(AdPayload adPayload, AdException error);
        void OnBannerClosed(AdPayload adPayload);
        void OnBannerImpression(AdPayload adPayload);
    }
}