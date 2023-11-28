using JetBrains.Annotations;

namespace SspnetSDK.ConsentManager.Unfiled
{
    public class ConsentBuilder
    {
        [CanBeNull] private string _appName;
        [CanBeNull] private string _bundle;
        [CanBeNull] private string _policyUrl;
        private bool _isGdpr;

        public ConsentBuilder AppendBundle(string value)
        {
            _bundle = value;
            return this;
        }

        public ConsentBuilder AppendPolicyURL(string value)
        {
            _policyUrl = value;
            return this;
        }

        public ConsentBuilder AppendName(string value)
        {
            _appName = value;
            return this;
        }

        public ConsentBuilder AppendGdpr(bool value)
        {
            _isGdpr = value;
            return this;
        }

        public string GetBundle()
        {
            return _bundle;
        }

        public string GetPolicyURL()
        {
            return _policyUrl;
        }

        public string GetAppName()
        {
            return _appName;
        }

        public bool GetGdpr()
        {
            return _isGdpr;
        }
    }
}