namespace SspnetSDK.Unfiled
{
    public class AdException
    {
        public readonly string Caused;
        public readonly string Description;
        public readonly string Message;

        public AdException(string description, string message, string caused)
        {
            Description = description;
            Message = message;
            Caused = caused;
        }
    }
}