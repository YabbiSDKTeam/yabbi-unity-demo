using System;

namespace SspnetSDK.Editor.Utils
{
    public class VersionUtils
    {
        public static int CompareVersions(string internalVersion, string latestVersion)
        {
            return new Version(internalVersion).CompareTo(new Version(latestVersion));
        }
    }
}