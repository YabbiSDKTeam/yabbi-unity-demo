namespace SspnetSDK.Unfiled
{
    public class AdException
    {
        public readonly string Description;
        public readonly string Message;
        public readonly string Caused;

        public AdException(string description,string message, string caused) {
            Description = description;
            Message = message;
            Caused = caused;
        }
    }
}