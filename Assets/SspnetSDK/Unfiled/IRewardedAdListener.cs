namespace SspnetSDK.Unfiled
{
    public interface IRewardedAdListener
    {
        void OnRewardedLoaded(AdPayload adPayload);
        void OnRewardedLoadFailed(AdPayload adPayload, AdException error);
        void OnRewardedShown(AdPayload adPayload);
        void OnRewardedShowFailed(AdPayload adPayload, AdException error);
        void OnRewardedClosed(AdPayload adPayload);
        void OnRewardedVideoStarted(AdPayload adPayload);
        void OnRewardedVideoCompleted(AdPayload adPayload);
        void OnUserRewarded(AdPayload adPayload);
    }
}