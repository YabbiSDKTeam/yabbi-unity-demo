using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEditor;
using SspnetSDK.Editor.NetworkManager.Data;

namespace SspnetSDK.Editor.NetworkManager
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class SspnetDependencyUtils
    {
        #region Constants

        public const string PluginRequest = "https://sdkapi.sspnet.tech/api/versions.json";
        public const string InternalResourcesPath = "Assets/SspnetSDK/Editor/InternalResources/";
        public const string NetworkConfigsPath = "Assets/SspnetSDK/Editor/NetworkConfigs/";
        public const string PackageName = "Name";
        public const string CurrentVersionHeader = "Current Version";
        public const string LatestVersionHeader = "Latest Version";
        public const string ActionHeader = "Action";
        public const string BoxStyle = "box";
        public const string ActionUpdate = "Update";
        public const string ActionImport = "Import";
        public const string ActionRemove = "Remove";
        public const string EmptyCurrentVersion = "   -  ";
        public const string Sspnet = "Sspnet";
        public const string Loading = "Loading...";
        public const string ProgressBarCancelled = "Progress bar canceled by the user";
        public const string iOS = "iOS";
        public const string Android = "Android";
        public const string SspnetNetworkDependencies = "Network Dependencies";
        public const string SspnetUnityPlugin = "Plugin";
        public const string SspnetCoreDependencies = "Core Dependencies";
        public const string SpecOpenDependencies = "<dependencies>\n";
        public const string SpecCloseDependencies = "</dependencies>";
        public const string XmlFileExtension = ".xml";
        public const string Dependencies = "Dependencies";
        public const string SspnetCore = "SspnetCore";

        #endregion
        
        public static void GuiHeaders(GUIStyle headerInfoStyle, GUILayoutOption btnFieldWidth)
        {
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
            {
                GUILayout.Button(PackageName, headerInfoStyle, GUILayout.Width(150));
                GUILayout.Space(25);
                GUILayout.Button(CurrentVersionHeader, headerInfoStyle, GUILayout.Width(120));
                GUILayout.Space(90);
                GUILayout.Button(LatestVersionHeader, headerInfoStyle);
                GUILayout.Button(ActionHeader, headerInfoStyle, btnFieldWidth);
                GUILayout.Button(string.Empty, headerInfoStyle, GUILayout.Width(5));
            }
        }

        public static SspnetInternalAdapter GetCoreDependency(SortedDictionary<string, SspnetInternalAdapter> items)
        {
            var key = SspnetCore.ToLower();
            if (items.TryGetValue(key, out var internalDependency))
            {
                return items[key];
            }
            return null;
        }

        public static FileInfo[] GetInternalDependencyPath()
        {
            try
            {
                var info = new DirectoryInfo(NetworkConfigsPath);
                var fileInfo = info.GetFiles();

                return fileInfo.Length <= 0 ? null : fileInfo.Where(val => !val.Name.Contains("meta")).ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        
        public static string GetAndroidDependencyVersion(string value)
        {
            string androidDependencyVersion = value.Substring(value.LastIndexOf(':') + 1);
            if (androidDependencyVersion.Contains("@aar"))
            {
                androidDependencyVersion = androidDependencyVersion.Substring(0,
                    androidDependencyVersion.IndexOf('@'));
            }

            return androidDependencyVersion;
        }
        
        public static string GetDependencyName(string value)
        {
            return value.Replace(NetworkConfigsPath, "").Replace("Dependencies", "").Replace(".xml", "").ToLower();
        }
        
        public static void ShowInternalErrorDialog(EditorWindow editorWindow, string message)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogError(message);
            var option = EditorUtility.DisplayDialog("Internal error",
                $"{message}.",
                "Ok");
            if (option)
            {
                editorWindow.Close();
            }
        }

        public static void FormatXml(string inputXml)
        {
            var document = new XmlDocument();
            document.Load(inputXml);
            using (var writer = new XmlTextWriter(inputXml, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.Indentation = 4;
                document.Save(writer);
            }
        }

        public static string GetConfigName(string value)
        {
            var configName = value.Replace(NetworkConfigsPath, string.Empty);
            return configName.Replace("Dependencies.xml", string.Empty);
        }

        public static string GetiOSContent(string path)
        {
            
            var iOSContent = string.Empty;
            try
            {
                var lines = File.ReadAllLines(path.Replace("Consentmanager","Consent"));
                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line)) continue;
            
                    if (line.Contains("<iosPods>"))
                    {
                        iOSContent += line + "\n";
                    }
            
                    if (line.Contains("<iosPod name="))
                    {
                        iOSContent += line + "\n";
                    }
            
                    if (line.Contains("<sources>"))
                    {
                        iOSContent += line + "\n";
                    }
            
                    if (line.Contains("<source>"))
                    {
                        iOSContent += line + "\n";
                    }
            
                    if (line.Contains("</sources>"))
                    {
                        iOSContent += line + "\n";
                    }
            
                    if (line.Contains("</iosPod>"))
                    {
                        iOSContent += line + "\n";
                    }
            
                    if (line.Contains("</iosPods>"))
                    {
                        iOSContent += line;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return iOSContent;
        }

        public static string GetAndroidContent(string path)
        {
            var androidContent = string.Empty;
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;

                if (line.Contains("<androidPackages>"))
                {
                    androidContent += line + "\n";
                }

                if (line.Contains("<androidPackage spec="))
                {
                    androidContent += line + "\n";
                }

                if (line.Contains("<repositories>"))
                {
                    androidContent += line + "\n";
                }

                if (line.Contains("<repository>"))
                {
                    androidContent += line + "\n";
                }

                if (line.Contains("</repositories>"))
                {
                    androidContent += line + "\n";
                }

                if (line.Contains("</androidPackages>"))
                {
                    androidContent += line;
                }
            }

            return androidContent;
        }
        
        public static void ShowInternalErrorDialog(EditorWindow editorWindow, string message, string debugLog)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogError(message);
            var option = EditorUtility.DisplayDialog("Internal error",
                $"{message}. Please contact support.",
                "Ok");
            if (option)
            {
                editorWindow.Close();
            }
        }

        public static string FormatName(string value)
        {
            return value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower();
        }

        public static void ReplaceInFile(
            string filePath, string searchText, string replaceText)
        {
            string contentString;
            using (var reader = new StreamReader(filePath))
            {
                contentString = reader.ReadToEnd();
                reader.Close();
            }

            contentString = Regex.Replace(contentString.Replace("\r", ""), searchText, replaceText);

            using (var writer = new StreamWriter(filePath))
            {
                writer.Write(contentString);
                writer.Close();
            }
        }
        
        public static int CompareVersion(string internalVersion, string latestVersion)
        {
            return new Version($"{internalVersion}.0").CompareTo(new Version($"{latestVersion}.0"));
            //
            // if (result > 0)
            //     Console.WriteLine("version1 is greater");
            // else if (result < 0)
            //     Console.WriteLine("version2 is greater");
            // else
            //     Console.WriteLine("versions are equal");
        }
    }
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            var wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            var wrapper = new Wrapper<T> {Items = array};
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            var wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        public static string fixJson(string value)
        {
            value = "{\"Items\":" + value + "}";
            return value;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}