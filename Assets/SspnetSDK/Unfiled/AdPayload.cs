namespace SspnetSDK.Unfiled
{
    public class AdPayload
    {
        public readonly string PlacementName;
        public readonly string Message;
        public readonly string Caused;

        public AdPayload(string placementName)
        {
            PlacementName = placementName;
        }
    }
}