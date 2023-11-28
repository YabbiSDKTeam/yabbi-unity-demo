using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.Serialization;

namespace SspnetSDK.Editor.NetworkManager.Data
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [Serializable]
    public class SspnetResponse
    {
        public PlatformResponse android;
        public PlatformResponse ios;
        public PlatformResponse unity;
    }
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [Serializable]
    public class PlatformResponse
    {
        public string version;
        public string min_version;
 		public string unity_content;
        public SspnetAdapter[] networks;
        
        public PlatformResponse(string version, string min_version, SspnetAdapter[] networks, string unity_content)
        {
            this.version = version;
            this.min_version = min_version;
            this.networks = networks;
			this.unity_content = unity_content;
        }
    }
    
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [Serializable]
    public class SspnetAdapter
    {
        public string name;
        public string version;
        public string min_version;
        public string unity_content;
        public string dependencyPath => $"{SspnetDependencyUtils.NetworkConfigsPath}{SspnetDependencyUtils.FormatName(name)}Dependencies.xml";

        public SspnetAdapter(string name, string version, string min_version, string unity_content)
        {
            this.name = name;
            this.version = version;
            this.min_version = min_version;
            this.unity_content = unity_content;
        }
        
        public SspnetAdapter(string name)
        {
            this.name = name;
        }
    }
    
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [Serializable]
    public class SspnetInternalAdapter
    {
        public string name;
        public string iosVersion;
        public string androidVersion;
        public string iosUnityContent;
        [FormerlySerializedAs("androidUnityInfo")] public string androidUnityContent;
        public string dependencyPath => $"{SspnetDependencyUtils.NetworkConfigsPath}{SspnetDependencyUtils.FormatName(name)}Dependencies.xml";

        public SspnetInternalAdapter(string name)
        {
            this.name = name;
        }
    }
}