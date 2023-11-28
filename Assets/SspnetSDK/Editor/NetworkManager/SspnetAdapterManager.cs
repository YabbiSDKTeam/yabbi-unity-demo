#if UNITY_2018_1_OR_NEWER
using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
using File = UnityEngine.Windows.File;
using UnityEngine.Networking;
using SspnetSDK.Editor.NetworkManager.Data;
using SspnetSDK.Editor.NetworkManager;
using marijnz.EditorCoroutines;
using Unity.VisualScripting;

#pragma warning disable 618

#pragma warning disable 612

namespace SspnetSDK.Editor.NetworkManager
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum PlatformSdk
    {
        Android,
        iOS
    }
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "NotAccessedVariable")]
    [SuppressMessage("ReSharper", "CollectionNeverQueried.Local")]
    public class SspnetAdapterManager : EditorWindow
    {
        public const string PLUGIN_VERSION = "1.0.0";

        #region Dictionaries
        
        private SortedDictionary<string, SspnetInternalAdapter> internalDependencies = new();
        
        #endregion

        #region GUIStyles
        
        private GUIStyle labelStyle;
        private GUIStyle headerInfoStyle;
        private GUIStyle packageInfoStyle;
        private readonly GUILayoutOption btnFieldWidth = GUILayout.Width(60);
        
        #endregion

        private static EditorCoroutines.EditorCoroutine coroutine;
        
        private float progress;
        private float loading;
        private WebClient downloader;

        private SspnetResponse _response;
        
        private bool _isPluginInfoReady;
        
        private Vector2 scrollPosition;

        private static string pluginUrl;

        public static void ShowSdkManager(string source)
        {
            pluginUrl = source;
            GetWindow(typeof(SspnetAdapterManager),
                true, "Dependency manager");
        }
        
        private void OnEnable()
        {
            loading = 0f;
            coroutine = this.StartCoroutine(GetSDKData());
        }
        
        private void UpdateWindow()
        {
            Reset();
            coroutine = this.StartCoroutine(GetSDKData());
            GUI.enabled = true;
            AssetDatabase.Refresh();
        }

        private void Reset()
        {
            internalDependencies = new SortedDictionary<string, SspnetInternalAdapter>();
            
            if (downloader != null)
            {
                downloader.CancelAsync();
                return;
            }

            if (coroutine != null)
                this.StopCoroutine(coroutine.routine);
            if (progress > 0)
                EditorUtility.ClearProgressBar();
            if (loading > 0)
                EditorUtility.ClearProgressBar();

            coroutine = null;
            downloader = null;

            loading = 0f;
            progress = 0f;
        }

        private IEnumerator GetSDKData()
        {
            yield return null;

            if (!EditorUtility.DisplayCancelableProgressBar(
                    "Dependency manager",
                SspnetDependencyUtils.Loading,
                80f))
            {
            }

            #region Internal

            if (SspnetDependencyUtils.GetInternalDependencyPath() != null)
            {
                foreach (var fileInfo in SspnetDependencyUtils.GetInternalDependencyPath())
                {
                    if (File.Exists(SspnetDependencyUtils.NetworkConfigsPath + fileInfo.Name))
                    {
                        GetInternalDependencies(SspnetDependencyUtils.NetworkConfigsPath + fileInfo.Name);
                    }
                }
            }

            #endregion
            
            #region Plugin
            
            using (var webRequest = UnityWebRequest.Get(SspnetDependencyUtils.PluginRequest))
            {
                yield return webRequest.SendWebRequest();
                var pages = SspnetDependencyUtils.PluginRequest.Split('/');
                var page = pages.Length - 1;
                if (webRequest.isNetworkError)
                {
                    Debug.Log(pages[page] + ": Error: " + webRequest.error);
                    SspnetDependencyUtils.ShowInternalErrorDialog(this, webRequest.error, string.Empty);
                }
                else
                {
                    if (string.IsNullOrEmpty(webRequest.downloadHandler.text))
                    {
                        SspnetDependencyUtils.ShowInternalErrorDialog(this, "Can't find plugin information",
                            string.Empty);
                        yield break;
                    }
            
                    var response = JsonUtility.FromJson<SspnetResponse>(webRequest.downloadHandler.text);
                    _response = response;
                    
                    if (_response == null)
                    {
                        SspnetDependencyUtils.ShowInternalErrorDialog(this, "Can't find plugin information",
                            string.Empty);
                        yield break;
                    }
                }
            }
            
            #endregion
            
            coroutine = null;
            
            _isPluginInfoReady = true;
            
            EditorUtility.ClearProgressBar();
        }

        private void GetInternalDependencies(string dependencyPath)
        {
            var networkDependency = new SspnetInternalAdapter(SspnetDependencyUtils.GetConfigName(dependencyPath));
            
            XmlUtilities.ParseXmlTextFileElements(dependencyPath,
                (reader, elementName, _, parentElementName, _) =>
                {
                    switch (elementName)
                    {
                        case "iosPod" when parentElementName == "iosPods":
                        {
                            var podName = reader.GetAttribute("name");
                            var version = reader.GetAttribute("version");
                            
                            if (podName == null || version == null) return false;
                            
                            networkDependency.iosVersion = version;
                            networkDependency.iosUnityContent = SspnetDependencyUtils.GetiOSContent(networkDependency.dependencyPath);
                            break;
                        }
                        case "androidPackage" when parentElementName == "androidPackages":
                        {
                            var specName = reader.GetAttribute("spec");
                            
                            if (specName == null) return false;
                            
                            networkDependency.androidVersion = SspnetDependencyUtils.GetAndroidDependencyVersion(specName);
                            networkDependency.androidUnityContent = SspnetDependencyUtils.GetAndroidContent(networkDependency.dependencyPath);
                            break;
                        }
                    }
                    return true;
                });
            
            networkDependency.name = SspnetDependencyUtils.GetDependencyName(dependencyPath);

            if (!string.IsNullOrEmpty(networkDependency.name))
            {
                internalDependencies.Add(networkDependency.name, networkDependency);
            }
        }

        private void Awake()
        {
            labelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };
            packageInfoStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Normal,
                fixedHeight = 18
            };

            headerInfoStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                fixedHeight = 18
            };

            Reset();
        }

        private void OnGUI()
        {
            minSize = new Vector2(700, 650);
            maxSize = new Vector2(2000, 2000);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition,
                false,
                false);
            GUILayout.BeginVertical();
            
            
            if (_isPluginInfoReady)
            {
                
                #region Plugin
                
                GUILayout.Space(10);
                EditorGUILayout.LabelField(SspnetDependencyUtils.SspnetUnityPlugin, labelStyle, GUILayout.Height(20));
                
                if (_response != null)
                {
                    using (new EditorGUILayout.VerticalScope(SspnetDependencyUtils.BoxStyle, GUILayout.Height(45)))
                    {
                        SspnetDependencyUtils.GuiHeaders(headerInfoStyle, btnFieldWidth);
                        SetAdapterUpdateInfo(
                            SspnetDependencyUtils.SspnetUnityPlugin,
                            PLUGIN_VERSION,
                            _response.unity.version,
                            "",
                            "",
                            false
                        );
                    }
                }
                
                #endregion
                
                #region CoreInfo
                
                var dependency = SspnetDependencyUtils.GetCoreDependency(internalDependencies);
                
                GUILayout.Space(10);
                EditorGUILayout.LabelField(SspnetDependencyUtils.SspnetCoreDependencies, labelStyle, GUILayout.Height(20));
                GUILayout.Space(10);
                EditorGUILayout.LabelField(SspnetDependencyUtils.iOS, labelStyle, GUILayout.Height(20));
                using (new EditorGUILayout.VerticalScope(SspnetDependencyUtils.BoxStyle, GUILayout.Height(45)))
                {
                    SspnetDependencyUtils.GuiHeaders(headerInfoStyle, btnFieldWidth);
                    SetAdapterUpdateInfo(
                        dependency.name,
                        dependency.iosVersion,
                        _response.ios.version,
                        dependency.iosUnityContent,
                        _response.ios.unity_content, 
                        false
                    );
                }
                
                EditorGUILayout.LabelField(SspnetDependencyUtils.Android, labelStyle, GUILayout.Height(20));
                using (new EditorGUILayout.VerticalScope(SspnetDependencyUtils.BoxStyle, GUILayout.Height(45)))
                {
                    SspnetDependencyUtils.GuiHeaders(headerInfoStyle, btnFieldWidth);
                    SetAdapterUpdateInfo(
                        dependency.name,
                        dependency.androidVersion,
                        _response.android.version,
                        dependency.androidUnityContent,
                        _response.android.unity_content, 
                        false
                    );
                }
                
                #endregion
                
                #region NetworksAdaptersInfo
            
                GUILayout.Space(10);
                EditorGUILayout.LabelField(SspnetDependencyUtils.SspnetNetworkDependencies, labelStyle, GUILayout.Height(20));
                GUILayout.Space(10);
            
                    if (_response != null && _response.ios.networks.Length > 0)
                    {
                        EditorGUILayout.LabelField(SspnetDependencyUtils.iOS, labelStyle, GUILayout.Height(20));
                        using (new EditorGUILayout.VerticalScope(SspnetDependencyUtils.BoxStyle, GUILayout.Height(45)))
                        {
                            SspnetDependencyUtils.GuiHeaders(headerInfoStyle, btnFieldWidth);
                            GuiAdaptersRows(PlatformSdk.iOS);
                        } 
                    }
                   
                    if (_response != null && _response.android.networks.Length > 0)
                    {
                        EditorGUILayout.LabelField(SspnetDependencyUtils.Android, labelStyle, GUILayout.Height(20));
                        using (new EditorGUILayout.VerticalScope(SspnetDependencyUtils.BoxStyle, GUILayout.Height(45)))
                        {
                            SspnetDependencyUtils.GuiHeaders(headerInfoStyle, btnFieldWidth);
                            GuiAdaptersRows(PlatformSdk.Android);
                        }
                    }
            
                #endregion
            }
            
            GUILayout.Space(5);
            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        
        private void GuiAdaptersRows(PlatformSdk platformSdk)
        {
            var networks = platformSdk == PlatformSdk.Android ? _response.android.networks : _response.ios.networks;

            foreach (var network in networks)
            {
                var latestVersion = network.version ?? "";
                
                if (internalDependencies.TryGetValue(network.name, out var internalDependency))
                {
                    var item = internalDependencies[network.name];
                    
                    var name = item?.name ?? "Undefiend";
                    var internalVersion = (platformSdk == PlatformSdk.Android ? item?.androidVersion : item?.iosVersion) ?? SspnetDependencyUtils.EmptyCurrentVersion;
                    var internalUnityContent = (platformSdk == PlatformSdk.Android ? item?.androidUnityContent : item?.iosUnityContent) ?? "";
                    
                    SetAdapterUpdateInfo(
                        SspnetDependencyUtils.FormatName(name),
                        internalVersion,
                        latestVersion,
                        internalUnityContent,
                        network.unity_content,
                        true
                    );
                }
                else
                {
                    SetAdapterUpdateInfo(
                        SspnetDependencyUtils.FormatName(network.name),
                        SspnetDependencyUtils.EmptyCurrentVersion,
                        latestVersion,
                        "",
                        network.unity_content,
                        true
                    );
                }
            }
        }
           
        
        private void SetAdapterUpdateInfo(string dependencyName, string internalVersion, string latestVersion,
            string internalContent, string latestContent, bool isNotRequired)
        {
            using (new EditorGUILayout.VerticalScope(SspnetDependencyUtils.BoxStyle))
            {
                using (new EditorGUILayout.HorizontalScope(GUILayout.Height(20)))
                {
                    GUILayout.Space(2);
                    
                    if (string.IsNullOrEmpty(dependencyName) || string.IsNullOrEmpty(internalVersion) ||
                        string.IsNullOrEmpty(latestVersion)) return;

                    var replacedName = dependencyName.Replace("sspnet", "");
                    EditorGUILayout.LabelField(new GUIContent
                    {
                        text = replacedName.First().ToString().ToUpper() + replacedName.Substring(1)
                    }, packageInfoStyle, GUILayout.Width(145));
                    GUILayout.Space(56);
                    GUILayout.Button(internalVersion, packageInfoStyle,GUILayout.Width(120));
                    GUILayout.Space(85);
                    GUILayout.Button(latestVersion, packageInfoStyle);
                    GUILayout.Space(15);
                    if (isNotRequired)
                    {
                        AddImportActionIfCan(dependencyName, internalVersion, latestContent);
                        AddRemoveActionIfCan(dependencyName, internalVersion, internalContent);
                    }

                    AddUpdatePluginActionIfCan(dependencyName, internalVersion, latestVersion);
                    AddUpdateActionIfCan(dependencyName, internalVersion, latestVersion, internalContent, latestContent);
                    GUILayout.Space(15);
                }
            }
        }
        
        private void AddRemoveActionIfCan(string dependencyName, string version, string content)
        {
            if (version != SspnetDependencyUtils.EmptyCurrentVersion) {

                if (GUILayout.Button(new GUIContent {text = SspnetDependencyUtils.ActionRemove}, btnFieldWidth))
                {
                    var path = $"{SspnetDependencyUtils.NetworkConfigsPath}{dependencyName}{SspnetDependencyUtils.Dependencies}{SspnetDependencyUtils.XmlFileExtension}";
                        
                    SspnetDependencyUtils.ReplaceInFile(path, content, "");
                    SspnetDependencyUtils.FormatXml(path);
                    
                    UpdateWindow();
                }
            }
        }

        private void AddImportActionIfCan(string name, string version, string content)
        {
            if (version == SspnetDependencyUtils.EmptyCurrentVersion) {
                Color defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.green;

                if (GUILayout.Button(new GUIContent {text = SspnetDependencyUtils.ActionImport}, btnFieldWidth))
                {
                    ImportConfig(name, content);
                }

                GUI.backgroundColor = defaultColor;
            }
        }
        
        private void AddUpdateActionIfCan(string dependencyName, string internalVersion, string latestVersion, string internalContent, string latestContent)
        {
            if (internalVersion != SspnetDependencyUtils.EmptyCurrentVersion && dependencyName != SspnetDependencyUtils.SspnetUnityPlugin)
            {
                var action = SspnetDependencyUtils.CompareVersion(internalVersion, latestVersion);
            
                if (action < 0) {
                    Color defaultColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.green;
                    
                    if (GUILayout.Button(new GUIContent { text = SspnetDependencyUtils.ActionUpdate }, btnFieldWidth))
                    {
                        UpdateDependency(dependencyName, internalContent, latestContent);
                        UpdateWindow();
                    }
                    
                    GUI.backgroundColor = defaultColor;
                }
                else
                {
                    GUI.enabled = false;
                    GUILayout.Button(new GUIContent {text = SspnetDependencyUtils.ActionUpdate}, btnFieldWidth);
                    GUI.enabled = true;
                }
            }
        }
        
        private void AddUpdatePluginActionIfCan(string dependencyName, string internalVersion, string latestVersion)
        {
            if (dependencyName == SspnetDependencyUtils.SspnetUnityPlugin && internalVersion != SspnetDependencyUtils.EmptyCurrentVersion )
            {
                var action = SspnetDependencyUtils.CompareVersion(internalVersion, latestVersion);
            
                if (action < 0) {
                    Color defaultColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.green;
                    
                    if (GUILayout.Button(new GUIContent { text = SspnetDependencyUtils.ActionUpdate }, btnFieldWidth))
                    {
                        this.StartCoroutine(DownloadUnityPlugin(latestVersion));
                    }
                    
                    GUI.backgroundColor = defaultColor;
                }
                else
                {
                    GUI.enabled = false;
                    GUILayout.Button(new GUIContent {text = SspnetDependencyUtils.ActionUpdate}, btnFieldWidth);
                    GUI.enabled = true;
                }
            }
        }
        
        private IEnumerator DownloadUnityPlugin(string version)
        {
            yield return null;
            var ended = false;
            var cancelled = false;
            Exception error = null;
            int oldPercentage = 0, newPercentage = 0;
            
            var path = Path.Combine("temp", "download");
            
            progress = 0.01f;
            
            downloader = new WebClient { Encoding = Encoding.UTF8 };
            downloader.DownloadProgressChanged += (sender, args) => { newPercentage = args.ProgressPercentage; };
            downloader.DownloadFileCompleted += (sender, args) =>
            {
                ended = true;
                cancelled = args.Cancelled;
                error = args.Error;
            };

            var source = pluginUrl.Replace("version", $"{version}");
            
            Debug.LogFormat("Downloading {0} to {1}", source, path);
            downloader.DownloadFileAsync(new Uri(source), path);
            
            
            while (!ended)
            {
                Repaint();
                var percentage = oldPercentage;
                yield return new WaitUntil(() => ended || newPercentage > percentage);
                oldPercentage = newPercentage;
                progress = oldPercentage / 100.0f;
            }
            
            if (error != null)
            {
                Debug.LogError(error);
                cancelled = true;
            }
            
            downloader = null;
            coroutine = null;
            progress = 0;
            EditorUtility.ClearProgressBar();
            if (!cancelled)
            {
                AssetDatabase.ImportPackage(path, true);
            }
            else
            {
                Debug.Log("Download terminated.");
            }
        }
        
        private void ImportConfig(string name, string content)
        {
            var path = $"{SspnetDependencyUtils.NetworkConfigsPath}{name}{SspnetDependencyUtils.Dependencies}{SspnetDependencyUtils.XmlFileExtension}";
            
            if (File.Exists(path))
            {
                UpdateDependency(name, SspnetDependencyUtils.SpecCloseDependencies, content + "\n" + SspnetDependencyUtils.SpecCloseDependencies);
            }
            else
            {
                using (TextWriter writer = new StreamWriter(path, false))
                {
                    writer.WriteLine(SspnetDependencyUtils.SpecOpenDependencies
                                     + content + "\n" + SspnetDependencyUtils.SpecCloseDependencies);
                    writer.Close();
                }

                SspnetDependencyUtils.FormatXml(path);
            }

            UpdateWindow();
        }
        
        private void UpdateDependency(string name, string previousContent, string latestContent)
        {
            var path = $"{SspnetDependencyUtils.NetworkConfigsPath}{name}{SspnetDependencyUtils.Dependencies}{SspnetDependencyUtils.XmlFileExtension}";
            
            if (!File.Exists(path))
            {
                SspnetDependencyUtils.ShowInternalErrorDialog(this, "Can't find config with path " + path);
            }
            else
            {
                string contentString;
                using (var reader = new StreamReader(path))
                {
                    contentString = reader.ReadToEnd();
                    reader.Close();
                }

                contentString = Regex.Replace(contentString, previousContent, latestContent);

                using (var writer = new StreamWriter(path))
                {
                    writer.Write(contentString);
                    writer.Close();
                }

                SspnetDependencyUtils.FormatXml(path);
            }
        }
    }
}

#endif