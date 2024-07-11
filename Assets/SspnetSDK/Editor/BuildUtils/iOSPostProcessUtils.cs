#if UNITY_IOS
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using SspnetSDK.Editor.InternalResources;
using UnityEngine;

#pragma warning disable 618

namespace SspnetSDK.Editor.BuildUtils
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "UnusedVariable")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    [SuppressMessage("ReSharper", "Unity.IncorrectMethodSignature")]
    public class iOSPostProcessUtils : MonoBehaviour
    {
        private const string suffix = ".framework";
        private const string minVersionToDisableBitcode = "14.0";

        [PostProcessBuild(41)]
        public static void updateInfoPlist(BuildTarget buildTarget, string buildPath)
        {
            var path = Path.Combine(buildPath, "Info.plist");
            AddNSUserTrackingUsageDescription(path);
            AddNSLocationWhenInUseUsageDescription(path);
            // AddSkAdNetworkIds(buildTarget, buildPath);
        }

        private static void AddSkAdNetworkIds(BuildTarget buildTarget, string buildPath)
        {
            if (string.IsNullOrEmpty(PlayerSettings.iOS.targetOSVersionString)) return;

            if (!SspnetSettings.Instance.IOSSkAdNetworkItems ||
                (SspnetSettings.Instance.IOSSkAdNetworkItemsList?.Count ?? 0) <= 0) return;

            if (buildTarget != BuildTarget.iOS) return;

            var plistPath = buildPath + "/Info.plist";
            var plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            PlistElementArray array = null;
            if (plist.root.values.ContainsKey(SspnetBuildUtils.KeySkAdNetworkItems))
                try
                {
                    PlistElement element;
                    plist.root.values.TryGetValue(SspnetBuildUtils.KeySkAdNetworkItems, out element);
                    if (element != null) array = element.AsArray();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    array = null;
                }
            else
                array = plist.root.CreateArray(SspnetBuildUtils.KeySkAdNetworkItems);

            if (array != null)
                foreach (var id in SspnetSettings.Instance.IOSSkAdNetworkItemsList)
                {
                    if (ContainsSkAdNetworkIdentifier(array, id)) continue;
                    var added = array.AddDict();
                    added.SetString(SspnetBuildUtils.KeySkAdNetworkID, id);
                }

            File.WriteAllText(plistPath, plist.WriteToString());
        }

        private static void AddKeyToPlist(string path, string key, string value)
        {
            var plist = new PlistDocument();
            plist.ReadFromFile(path);
            plist.root.SetString(key, value);
            File.WriteAllText(path, plist.WriteToString());
        }

        private static bool CheckContainsKey(string path, string key)
        {
            string contentString;
            using (var reader = new StreamReader(path))
            {
                contentString = reader.ReadToEnd();
                reader.Close();
            }

            return contentString.Contains(key);
        }

        private static void AddNSUserTrackingUsageDescription(string path)
        {
            if (!SspnetSettings.Instance.NSUserTrackingUsageDescription) return;
            if (!CheckContainsKey(path, "NSUserTrackingUsageDescription"))
                AddKeyToPlist(path, "NSUserTrackingUsageDescription",
                    "$(PRODUCT_NAME)" + " " +
                    "needs your advertising identifier to provide personalised advertising experience tailored to you.");
        }

        private static void AddNSLocationWhenInUseUsageDescription(string path)
        {
            if (!SspnetSettings.Instance.NSLocationWhenInUseUsageDescription) return;
            if (!CheckContainsKey(path, "NSLocationWhenInUseUsageDescription"))
                AddKeyToPlist(path, "NSLocationWhenInUseUsageDescription",
                    "$(PRODUCT_NAME)" + " " +
                    "needs your location for analytics and advertising purposes.");
        }

        private static void ReplaceInFile(
            string filePath, string searchText, string replaceText)
        {
            string contentString;
            using (var reader = new StreamReader(filePath))
            {
                contentString = reader.ReadToEnd();
                reader.Close();
            }

            contentString = Regex.Replace(contentString, searchText, replaceText);

            using (var writer = new StreamWriter(filePath))
            {
                writer.Write(contentString);
                writer.Close();
            }
        }

        private static readonly string[] frameworkList =
        {
            "CoreLocation",
            "SafariServices",
            "UIKit",
            "WebKit"
        };

        private static readonly string[] weakFrameworkList =
        {
            "AppTrackingTransparency"
        };

        private static readonly string[] platformLibs =
        {
            "libc++.dylib",
            "libz.dylib",
            "libsqlite3.dylib",
            "libxml2.2.dylib"
        };

        public static void PrepareProject(string buildPath)
        {
            Debug.Log("preparing your xcode project");
            var projectPath = PBXProject.GetPBXProjectPath(buildPath);
            var project = new PBXProject();

            project.ReadFromString(File.ReadAllText(projectPath));

#if UNITY_2019_3_OR_NEWER
            var target = project.GetUnityMainTargetGuid();
            var unityFrameworkTarget = project.GetUnityFrameworkTargetGuid();
#else
            var target = project.TargetGuidByName("Unity-iPhone");
#endif

            AddProjectFrameworks(frameworkList, project, target, false);
            AddProjectFrameworks(weakFrameworkList, project, target, true);
            AddProjectLibs(platformLibs, project, target);
            project.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");

            var xcodeVersion = SspnetBuildUtils.getXcodeVersion();
            if (xcodeVersion == null ||
                SspnetBuildUtils.compareVersions(xcodeVersion, minVersionToDisableBitcode) >= 0)
                project.SetBuildProperty(project.ProjectGuid(), "ENABLE_BITCODE", "NO");

            project.AddBuildProperty(target, "LIBRARY_SEARCH_PATHS", "$(SRCROOT)/Libraries");
            project.AddBuildProperty(target, "LIBRARY_SEARCH_PATHS", "$(TOOLCHAIN_DIR)/usr/lib/swift/$(PLATFORM_NAME)");
#if UNITY_2019_3_OR_NEWER
            project.AddBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
            project.AddBuildProperty(unityFrameworkTarget, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
#else
            project.AddBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
#endif
            project.AddBuildProperty(target, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
            project.SetBuildProperty(target, "SWIFT_VERSION", "4.0");

            File.WriteAllText(projectPath, project.WriteToString());
        }

        private static void AddProjectFrameworks(IEnumerable<string> frameworks, PBXProject project, string target,
            bool weak)
        {
            foreach (var framework in frameworks)
                if (!project.ContainsFramework(target, framework))
                    project.AddFrameworkToProject(target, framework + suffix, weak);
        }

        private static void AddProjectLibs(IEnumerable<string> libs, PBXProject project, string target)
        {
            foreach (var lib in libs)
            {
                var libGUID = project.AddFile("usr/lib/" + lib, "Libraries/" + lib, PBXSourceTree.Sdk);
                project.AddFileToBuild(target, libGUID);
            }
        }

        private static void CopyAndReplaceDirectory(string srcPath, string dstPath)
        {
            if (Directory.Exists(dstPath)) Directory.Delete(dstPath);

            if (File.Exists(dstPath)) File.Delete(dstPath);

            Directory.CreateDirectory(dstPath);

            foreach (var file in Directory.GetFiles(srcPath))
                if (!file.Contains(".meta"))
                    File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));

            foreach (var dir in Directory.GetDirectories(srcPath))
                CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
        }

        private static bool ContainsSkAdNetworkIdentifier(PlistElementArray skAdNetworkItemsArray, string id)
        {
            foreach (var elem in skAdNetworkItemsArray.values)
                try
                {
                    PlistElement value;
                    var identifierExists = elem.AsDict().values
                        .TryGetValue(SspnetBuildUtils.KeySkAdNetworkID, out value);

                    if (identifierExists && value.AsString().Equals(id)) return true;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }

            return false;
        }
    }
}
#endif