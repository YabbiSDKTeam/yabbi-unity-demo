namespace SspnetSDK.Unfiled
{
    public interface IInitializationListener
    {
        void OnInitializeSuccess();
        void OnInitializeFailed(AdException error);
    }
}