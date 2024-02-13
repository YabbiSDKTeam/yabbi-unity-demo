namespace SspnetSDK.Unfiled
{
    public interface IRewardedAdListener
    {
        void OnRewardedLoaded(AdPayload adPayload);
        void OnRewardedLoadFailed(AdPayload adPayload, AdException error);
        void OnRewardedShown(AdPayload adPayload);
        void OnRewardedShowFailed(AdPayload adPayload, AdException error);
        void OnRewardedFinished(AdPayload adPayload);
        void OnRewardedClosed(AdPayload adPayload);
    }
}