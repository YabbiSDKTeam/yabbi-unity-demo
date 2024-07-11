using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SspnetSDK.Editor.BuildUtils
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "ShiftExpressionRealShiftCountIsZero")]
    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
    public static class SspnetBuildUtils
    {
        private const BindingFlags PublicStaticFlags = BindingFlags.Public | BindingFlags.Static;
        public const string KeySkAdNetworkItems = "SKAdNetworkItems";
        public const string KeySkAdNetworkID = "SKAdNetworkIdentifier";
        public const string GADApplicationIdentifier = "GADApplicationIdentifier";
        public const string NSUserTrackingUsageDescriptionKey = "NSUserTrackingUsageDescription";

        public const string NSUserTrackingUsageDescription =
            "This identifier will be used to deliver personalized ads to you";

        #region Optional Android Permissions

        public const string CoarseLocation = "android.permission.ACCESS_COARSE_LOCATION";
        public const string FineLocation = "android.permission.ACCESS_FINE_LOCATION";

        #endregion

        public static int CompareVersions(string v1, string v2)
        {
            var re = new Regex(@"\d+(\.\d+)+");
            var match1 = re.Match(v1);
            var match2 = re.Match(v2);
            return new Version(match1.ToString()).CompareTo(new Version(match2.ToString()));
        }

        public static string GetXcodeVersion()
        {
            string profilerOutput = null;
            try
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo("system_profiler", "SPDeveloperToolsDataType | grep \"Xcode:\"")
                    {
                        CreateNoWindow = false, RedirectStandardOutput = true, UseShellExecute = false
                    }
                };
                p.Start();
                p.WaitForExit();
                profilerOutput = p.StandardOutput.ReadToEnd();
                var re = new Regex(@"Xcode: (?<version>\d+(\.\d+)+)");
                var m = re.Match(profilerOutput);
                if (m.Success) profilerOutput = m.Groups["version"].Value;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }

            return profilerOutput;
        }

        public static bool isGradleEnabled()
        {
            var isGradleEnabledVal = false;
            var androidBuildSystem =
                typeof(EditorUserBuildSettings).GetProperty("androidBuildSystem", PublicStaticFlags);
            if (androidBuildSystem == null) return isGradleEnabledVal;
            var gradle = Enum.Parse(androidBuildSystem.PropertyType, "Gradle");
            isGradleEnabledVal = androidBuildSystem.GetValue(null, null).Equals(gradle);

            return isGradleEnabledVal;
        }

        public static bool isGradleAvailable()
        {
            var androidBuildSystem =
                typeof(EditorUserBuildSettings).GetProperty("androidBuildSystem", PublicStaticFlags);
            return androidBuildSystem != null;
        }

        public static string combinePaths(params string[] paths)
        {
            var result = paths[0];
            for (var i = 1; i < paths.Length; i++) result = Path.Combine(result, paths[i]);

            return result;
        }

        public static string absolute2Relative(string absolutepath)
        {
            var relativepath = absolutepath;
            if (absolutepath.StartsWith(Application.dataPath, StringComparison.Ordinal))
                relativepath = "Assets" + absolutepath.Substring(Application.dataPath.Length);

            return relativepath;
        }

        public static XmlNode XmlFindChildNode(XmlNode parent, string name)
        {
            var curr = parent.FirstChild;
            while (curr != null)
            {
                if (curr.Name.Equals(name)) return curr;

                curr = curr.NextSibling;
            }

            return null;
        }

        public static int compareVersions(string v1, string v2)
        {
            var re = new Regex(@"\d+(\.\d+)+");
            var match1 = re.Match(v1);
            var match2 = re.Match(v2);
            return new Version(match1.ToString()).CompareTo(new Version(match2.ToString()));
        }

        public static void enableGradleBuildSystem()
        {
            var androidBuildSystem =
                typeof(EditorUserBuildSettings).GetProperty("androidBuildSystem", PublicStaticFlags);
            if (androidBuildSystem == null) return;
            var gradle = Enum.Parse(androidBuildSystem.PropertyType, "Gradle");
            androidBuildSystem.SetValue(null, gradle, null);
        }

        public static string getXcodeVersion()
        {
            string profilerOutput = null;
            try
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo("system_profiler", "SPDeveloperToolsDataType | grep \"Xcode:\"")
                    {
                        CreateNoWindow = false, RedirectStandardOutput = true, UseShellExecute = false
                    }
                };
                p.Start();
                p.WaitForExit();
                profilerOutput = p.StandardOutput.ReadToEnd();
                var re = new Regex(@"Xcode: (?<version>\d+(\.\d+)+)");
                var m = re.Match(profilerOutput);
                if (m.Success) profilerOutput = m.Groups["version"].Value;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message);
            }

            return profilerOutput;
        }
    }
}