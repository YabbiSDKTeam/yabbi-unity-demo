using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.Serialization;

namespace SspnetSDK.Editor.NetworkManager.Data
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
        
        public PlatformSdkInfo(string version, string min_version, List<AdapterSdkInfo> networks, string unity_content)
        {
            this.version = version;
            this.min_version = min_version;
            this.networks = networks;
			this.unity_content = unity_content;
        }
        
        public PlatformSdkInfo()
        {
            networks = new List<AdapterSdkInfo>();
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

       // public string dependencyPath => $"{SspnetDependencyUtils.NetworkConfigsPath}{SspnetDependencyUtils.FormatName(name)}Dependencies.xml";
   
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
   }
   
   
   //  
   //  
   //  [SuppressMessage("ReSharper", "InconsistentNaming")]
   //  [Serializable]
   //  public class SspnetInternalAdapter
   //  {
   //      public string name;
   //      public string iosVersion;
   //      public string androidVersion;
   //      public string iosUnityContent;
   //      [FormerlySerializedAs("androidUnityInfo")] public string androidUnityContent;
   //      public string dependencyPath => $"{SspnetDependencyUtils.NetworkConfigsPath}{SspnetDependencyUtils.FormatName(name)}Dependencies.xml";
   //
   //      public SspnetInternalAdapter(string name)
   //      {
   //          this.name = name;
   //      }
   //  }
}