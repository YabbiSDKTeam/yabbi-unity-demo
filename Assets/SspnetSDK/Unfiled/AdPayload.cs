namespace SspnetSDK.Unfiled
{
    public class AdPayload
    {
        public readonly string Caused;
        public readonly string Message;
        public readonly string PlacementName;

        public AdPayload(string placementName)
        {
            PlacementName = placementName;
        }
    }
}