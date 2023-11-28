namespace SspnetSDK.Unfiled
{
    public interface IRewardedAdListener
    {
        void OnRewardedLoaded();
        void OnRewardedLoadFailed(AdException error);
        void OnRewardedShown();
        void OnRewardedShowFailed(AdException error);
        void OnRewardedFinished();
        void OnRewardedClosed();
    }
}