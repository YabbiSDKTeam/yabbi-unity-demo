using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.Serialization;

namespace SspnetSDK.Editor.Models
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [Serializable]
    public class SdkInfo
    {
        public PlatformSdkInfo android;
        public PlatformSdkInfo ios;
        public PlatformSdkInfo unity;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [Serializable]
    public class PlatformSdkInfo
    {
        public string version;
        public string min_version;
        public string unity_content;
        public List<AdapterSdkInfo> networks;
        public List<AdapterSdkInfo> adapters;

        public PlatformSdkInfo()
        {
            networks = new List<AdapterSdkInfo>();
            adapters = new List<AdapterSdkInfo>();
        }
    }


    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [Serializable]
    public class AdapterSdkInfo
    {
        public string name;
        public string version;
        public string min_version;
        public string unity_content;
        public List<AdapterSdkInfo> adapters;

        public AdapterSdkInfo(string name, string version, string min_version, string unity_content)
        {
            this.name = name;
            this.version = version;
            this.min_version = min_version;
            this.unity_content = unity_content;
        }

        public AdapterSdkInfo(string name)
        {
            this.name = name;
            adapters = new List<AdapterSdkInfo>();
        }

        public AdapterSdkInfo copyWithVersion(string vers)
        {
            return new AdapterSdkInfo(
                name,
                vers,
                min_version,
                unity_content
            );
        }

        public AdapterSdkInfo copyWithContent(string unity_content)
        {
            return new AdapterSdkInfo(
                name,
                version,
                min_version,
                unity_content
            );
        }
    }
}