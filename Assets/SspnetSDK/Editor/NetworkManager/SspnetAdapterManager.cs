#if UNITY_2018_1_OR_NEWER
using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEditor;
using UnityEngine;
using File = UnityEngine.Windows.File;
using UnityEngine.Networking;
using SspnetSDK.Editor.NetworkManager.Data;
using marijnz.EditorCoroutines;
using static System.String;

#pragma warning disable 618

#pragma warning disable 612

namespace SspnetSDK.Editor.NetworkManager
{
    public enum PlatformSdk
    {
        Android,
        IOS
    }

    //
    public class SspnetAdapterManager : EditorWindow
    {
        private const string PluginVersion = "1.2.0";

        #region GUIStyles

        private GUIStyle _labelStyle;
        private GUIStyle _headerInfoStyle;
        private GUIStyle _packageInfoStyle;
        private readonly GUILayoutOption _btnFieldWidth = GUILayout.Width(60);

        #endregion

        private static EditorCoroutines.EditorCoroutine _coroutine;

        private float _progress;
        private float _loading;
        private WebClient _downloader;

        private SdkInfo _sdkInfo;
        private SdkInfo _localSdkInfo;


        private Vector2 _scrollPosition;

        private static string _pluginUrl;

        public static void ShowSdkManager(string source)
        {
            _pluginUrl = source;
            GetWindow(typeof(SspnetAdapterManager),
                true, "Dependency manager");
        }

        private void OnEnable()
        {
            _loading = 0f;
            _coroutine = this.StartCoroutine(GetSDKData());
        }

        private void UpdateWindow()
        {
            Reset();
            _coroutine = this.StartCoroutine(GetSDKData());
            GUI.enabled = true;
            AssetDatabase.Refresh();
        }

        private void Reset()
        {
            _localSdkInfo = null;

            if (_downloader != null)
            {
                _downloader.CancelAsync();
                return;
            }

            if (_coroutine != null)
                this.StopCoroutine(_coroutine.routine);
            if (_progress > 0)
                EditorUtility.ClearProgressBar();
            if (_loading > 0)
                EditorUtility.ClearProgressBar();

            _coroutine = null;
            _downloader = null;

            _loading = 0f;
            _progress = 0f;
        }

        private IEnumerator GetSDKData()
        {
            yield return null;

            if (!EditorUtility.DisplayCancelableProgressBar("Dependency manager", SspnetDependencyUtils.Loading, 80f))
            {
            }

            #region Plugin

            using (var webRequest = UnityWebRequest.Get(SspnetDependencyUtils.PluginRequest))
            {
                yield return webRequest.SendWebRequest();
                var pages = SspnetDependencyUtils.PluginRequest.Split('/');
                var page = pages.Length - 1;
                if (webRequest.isNetworkError)
                {
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    SspnetDependencyUtils.ShowInternalErrorDialog(this, webRequest.error, Empty);
                }
                else
                {
                    if (IsNullOrEmpty(webRequest.downloadHandler.text))
                    {
                        SspnetDependencyUtils.ShowInternalErrorDialog(this, "Can't find plugin information", Empty);
                        yield break;
                    }

                    var response = JsonUtility.FromJson<SdkInfo>(webRequest.downloadHandler.text);
                    _sdkInfo = response;

                    if (_sdkInfo == null)
                    {
                        SspnetDependencyUtils.ShowInternalErrorDialog(this, "Can't find plugin information", Empty);
                        yield break;
                    }
                }
            }

            #endregion

            #region Internal

            var networkDependencies = GetLocalDependencies(SspnetDependencyUtils.NetworkConfigsPath);

            var info = new SdkInfo();
            var androidPlatformSdkInfo = new PlatformSdkInfo();
            var iosPlatformSdkInfo = new PlatformSdkInfo();

            if (networkDependencies != null)
            {
                foreach (var networkDependencyFile in networkDependencies)
                {
                    var path = SspnetDependencyUtils.NetworkConfigsPath + networkDependencyFile.Name;
                    if (!File.Exists(path)) continue;
                    if (path.ToLower().Contains("sspnet") && path.ToLower().Contains("core"))
                    {
                        var iosAdapterSdkInfo = GetAdapterSdkInfo(path, SspnetDependencyUtils.NetworkConfigsPath,
                            PlatformSdk.IOS);
                        var androidAdapterSdkInfo = GetAdapterSdkInfo(path, SspnetDependencyUtils.NetworkConfigsPath,
                            PlatformSdk.Android);


                        androidPlatformSdkInfo.version = androidAdapterSdkInfo.version;
                        androidPlatformSdkInfo.min_version = androidAdapterSdkInfo.min_version;
                        androidPlatformSdkInfo.unity_content = androidAdapterSdkInfo.unity_content;

                        iosPlatformSdkInfo.version = iosAdapterSdkInfo.version;
                        iosPlatformSdkInfo.min_version = iosAdapterSdkInfo.min_version;
                        iosPlatformSdkInfo.unity_content = iosAdapterSdkInfo.unity_content;
                    }
                    else
                    {
                        var iosAdapterSdkInfo = GetAdapterSdkInfo(path, SspnetDependencyUtils.NetworkConfigsPath,
                            PlatformSdk.IOS);
                        var androidAdapterSdkInfo = GetAdapterSdkInfo(path, SspnetDependencyUtils.NetworkConfigsPath,
                            PlatformSdk.Android);

                        var adapterName = GetAdapterName(path, SspnetDependencyUtils.NetworkConfigsPath);

                        var externalNetworkDependencies =
                            GetLocalDependencies(SspnetDependencyUtils.ExternalNetworkDependencies);

                        if (externalNetworkDependencies != null)
                        {
                            var externalNetworks =
                                externalNetworkDependencies.Where(value =>
                                    value.Name.ToLower().Contains(adapterName.ToLower()));

                            foreach (var fileInfo in externalNetworks)
                            {
                                var externalDependencyPath =
                                    SspnetDependencyUtils.ExternalNetworkDependencies + fileInfo.Name;

                                var androidExternalAdapterSdkInfo = GetAdapterSdkInfo(externalDependencyPath,
                                    SspnetDependencyUtils.ExternalNetworkDependencies, PlatformSdk.Android);
                                var iOSExternalAdapterSdkInfo = GetAdapterSdkInfo(externalDependencyPath,
                                    SspnetDependencyUtils.ExternalNetworkDependencies, PlatformSdk.IOS);

                                iosAdapterSdkInfo.adapters.Add(iOSExternalAdapterSdkInfo);
                                androidAdapterSdkInfo.adapters.Add(androidExternalAdapterSdkInfo);
                            }
                        }

                        androidPlatformSdkInfo.networks.Add(androidAdapterSdkInfo);
                        iosPlatformSdkInfo.networks.Add(iosAdapterSdkInfo);
                    }
                }
            }

            info.android = androidPlatformSdkInfo;
            info.ios = iosPlatformSdkInfo;
            _localSdkInfo = info;

            #endregion

            _coroutine = null;

            EditorUtility.ClearProgressBar();
        }

        private static FileInfo[] GetLocalDependencies(string path)
        {
            try
            {
                var info = new DirectoryInfo(path);
                var fileInfo = info.GetFiles();
                return fileInfo.Length <= 0 ? null : fileInfo.Where(val => !val.Name.Contains("meta")).ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static AdapterSdkInfo GetAdapterSdkInfo(string path, string oldValue, PlatformSdk platformSdk)
        {
            var sdkInfo = new AdapterSdkInfo(GetAdapterName(path, oldValue));

            XmlUtilities.ParseXmlTextFileElements(path,
                (reader, elementName, _, parentElementName, _) =>
                {
                    switch (platformSdk)
                    {
                        case PlatformSdk.IOS when elementName == "iosPod" && parentElementName == "iosPods":
                        {
                            var podName = reader.GetAttribute("name");
                            var version = reader.GetAttribute("version");

                            if (podName == null || version == null) return false;

                            if (sdkInfo.version != null) break;
                            sdkInfo.version = version;
                            sdkInfo.unity_content = SspnetDependencyUtils.GetiOSContent(path);
                            break;
                        }
                        case PlatformSdk.Android when elementName == "androidPackage" &&
                                                      parentElementName == "androidPackages":
                        {
                            var specName = reader.GetAttribute("spec");

                            if (specName == null) return false;

                            if (sdkInfo.version != null) break;
                            sdkInfo.version = SspnetDependencyUtils.GetAndroidDependencyVersion(specName);
                            sdkInfo.unity_content = SspnetDependencyUtils.GetAndroidContent(path);
                            break;
                        }
                    }

                    return true;
                });
            return sdkInfo;
        }

        private static string GetAdapterName(string value, string oldValue)
        {
            var configName = value.Replace(oldValue, Empty);
            return configName.Replace("Dependencies.xml", Empty).ToLower();
        }

        private bool IsSdkInfoReady()
        {
            return _sdkInfo != null && _localSdkInfo != null;
        }


        private void Awake()
        {
            _labelStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };
            _packageInfoStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Normal,
                fixedHeight = 18
            };

            _headerInfoStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold,
                fixedHeight = 18
            };

            Reset();
        }

        private static void SetDependenciesHeader(GUIStyle headerInfoStyle, GUILayoutOption btnFieldWidth)
        {
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
            {
                GUILayout.Button("Name", headerInfoStyle, GUILayout.Width(150));
                GUILayout.Space(25);
                GUILayout.Button("Current Version", headerInfoStyle, GUILayout.Width(120));
                GUILayout.Space(90);
                GUILayout.Button("Latest Version", headerInfoStyle);
                GUILayout.Button("Action", headerInfoStyle, btnFieldWidth);
                GUILayout.Button(Empty, headerInfoStyle, GUILayout.Width(5));
            }
        }

        private void OnGUI()
        {
            if (!IsSdkInfoReady()) return;

            minSize = new Vector2(700, 650);
            maxSize = new Vector2(2000, 2000);
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);
            GUILayout.BeginVertical();

            #region Plugin

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Plugin", _labelStyle, GUILayout.Height(20));

            SetDependenciesHeader(_headerInfoStyle, _btnFieldWidth);
            using (new EditorGUILayout.VerticalScope(SspnetDependencyUtils.BoxStyle))
            {
                SetDependencyRow("Plugin", PluginVersion, _sdkInfo.unity.version,
                    () => AddUpdatePluginActionIfCan(PluginVersion, _sdkInfo.unity.version));
            }

            #endregion

            #region Android

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Android", _labelStyle, GUILayout.Height(20));
            GUILayout.Space(5);

            SetDependenciesHeader(_headerInfoStyle, _btnFieldWidth);

            using (new EditorGUILayout.VerticalScope(SspnetDependencyUtils.BoxStyle))
            {
                SetDependencyRow("Core", _localSdkInfo.android.version, _sdkInfo.android.version,
                    () => AddUpdateCoreActionIfCan(PlatformSdk.Android));
            }


            foreach (var adapterSdkInfo in _sdkInfo.android.networks)
            {
                if (_localSdkInfo == null) continue;
                var localAdapterSdkInfoEnum = _localSdkInfo.android.networks.Where(value =>
                        string.Equals(value?.name, adapterSdkInfo?.name, StringComparison.CurrentCultureIgnoreCase))
                    .ToList();

                if (localAdapterSdkInfoEnum.Any())
                {
                    var localAdapterSdkInfo = localAdapterSdkInfoEnum.First();
                    SetDependencyRow(adapterSdkInfo.name, localAdapterSdkInfo.version, adapterSdkInfo.version,
                        () => AddDependencyActions(localAdapterSdkInfo, adapterSdkInfo,
                            CreateDependencyPath(adapterSdkInfo, null)));
                    if (localAdapterSdkInfo.version == null) continue;
                    foreach (var adapter in adapterSdkInfo.adapters)
                    {
                        var localExternalAdapterSdkInfoEnum = localAdapterSdkInfo.adapters.Where(value =>
                            string.Equals(value.name.Replace(adapterSdkInfo.name, ""), adapter.name,
                                StringComparison.CurrentCultureIgnoreCase)).ToList();

                        if (localExternalAdapterSdkInfoEnum.Any())
                        {
                            var localExternalAdapterSdkInfo = localExternalAdapterSdkInfoEnum.First();
                            SetDependencyRow(adapterSdkInfo.name + "-" + adapter.name,
                                localExternalAdapterSdkInfo.version, adapter.version,
                                () => AddDependencyActions(localExternalAdapterSdkInfo, adapter,
                                    CreateDependencyPath(adapter, adapterSdkInfo)));
                        }
                        else
                        {
                            SetDependencyRow(adapterSdkInfo.name + "-" + adapter.name, "", adapter.version,
                                () => AddImportActionIfCan(adapter, CreateDependencyPath(adapter, adapterSdkInfo)));
                        }
                    }
                }
                else
                {
                    SetDependencyRow(adapterSdkInfo.name, "", adapterSdkInfo.version,
                        () => AddImportActionIfCan(adapterSdkInfo, CreateDependencyPath(adapterSdkInfo, null)));
                }
            }

            #endregion

            #region iOS

            GUILayout.Space(10);
            EditorGUILayout.LabelField("iOS", _labelStyle, GUILayout.Height(20));
            GUILayout.Space(5);

            SetDependenciesHeader(_headerInfoStyle, _btnFieldWidth);

            if (_localSdkInfo != null)
            {
                using (new EditorGUILayout.VerticalScope(SspnetDependencyUtils.BoxStyle))
                {
                    SetDependencyRow("Core", _localSdkInfo.ios.version, _sdkInfo.ios.version,
                        () => AddUpdateCoreActionIfCan(PlatformSdk.IOS));
                }
            }

            foreach (var adapterSdkInfo in _sdkInfo.ios.networks)
            {
                if (_localSdkInfo == null) continue;
                var localAdapterSdkInfoEnum = _localSdkInfo.ios.networks.Where(value =>
                    string.Equals(value.name, adapterSdkInfo.name, StringComparison.CurrentCultureIgnoreCase)).ToList();

                if (localAdapterSdkInfoEnum.Any())
                {
                    var localAdapterSdkInfo = localAdapterSdkInfoEnum.First();
                    SetDependencyRow(adapterSdkInfo.name, localAdapterSdkInfo.version, adapterSdkInfo.version,
                        () => AddDependencyActions(localAdapterSdkInfo, adapterSdkInfo,
                            CreateDependencyPath(adapterSdkInfo, null)));
                    if (localAdapterSdkInfo.version == null) continue;
                    foreach (var adapter in adapterSdkInfo.adapters)
                    {
                        var localExternalAdapterSdkInfoEnum = localAdapterSdkInfo.adapters.Where(value =>
                            string.Equals(value.name.Replace(adapterSdkInfo.name, ""), adapter.name,
                                StringComparison.CurrentCultureIgnoreCase)).ToList();

                        if (localExternalAdapterSdkInfoEnum.Any())
                        {
                            var localExternalAdapterSdkInfo = localExternalAdapterSdkInfoEnum.First();
                            SetDependencyRow(adapterSdkInfo.name + "-" + adapter.name,
                                localExternalAdapterSdkInfo.version, adapter.version,
                                () => AddDependencyActions(localExternalAdapterSdkInfo, adapter,
                                    CreateDependencyPath(adapter, adapterSdkInfo)));
                        }
                        else
                        {
                            SetDependencyRow(adapterSdkInfo.name + "-" + adapter.name, "", adapter.version,
                                () => AddImportActionIfCan(adapter, CreateDependencyPath(adapter, adapterSdkInfo)));
                        }
                    }
                }
                else
                {
                    SetDependencyRow(adapterSdkInfo.name, "", adapterSdkInfo.version,
                        () => AddImportActionIfCan(adapterSdkInfo, CreateDependencyPath(adapterSdkInfo, null)));
                }
            }

            #endregion

            GUILayout.Space(5);
            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private static string CreateDependencyPath(AdapterSdkInfo adapterSdkInfo, AdapterSdkInfo parentSdkInfo)
        {
            if (parentSdkInfo == null)
            {
                var adapterName = adapterSdkInfo.name.First().ToString().ToUpper() + adapterSdkInfo.name[1..];
                return SspnetDependencyUtils.NetworkConfigsPath + adapterName + "Dependencies" + ".xml";
            }
            else
            {
                var adapterName = adapterSdkInfo.name.Replace(parentSdkInfo.name, "");
                var formatted = adapterName.First().ToString().ToUpper() + adapterName[1..];
                var parentAdapterName = parentSdkInfo.name.First().ToString().ToUpper() + parentSdkInfo.name[1..];
                return SspnetDependencyUtils.ExternalNetworkDependencies + "/" + parentAdapterName + formatted +
                       "Dependencies" + ".xml";
            }
        }


        private void SetDependencyRow(string dependencyName, string localVersion, string latestVersion, Action action)
        {
            using (new EditorGUILayout.VerticalScope(SspnetDependencyUtils.BoxStyle))
            {
                using (new EditorGUILayout.HorizontalScope(GUILayout.Height(20)))
                {
                    GUILayout.Space(2);

                    EditorGUILayout.LabelField(new GUIContent
                    {
                        text = dependencyName.First().ToString().ToUpper() + dependencyName[1..]
                    }, _packageInfoStyle, GUILayout.Width(145));
                    GUILayout.Space(56);
                    if (localVersion == null || !localVersion.Any())
                    {
                        GUILayout.Button("   -   ", _packageInfoStyle, GUILayout.Width(120));
                    }
                    else
                    {
                        GUILayout.Button(localVersion, _packageInfoStyle, GUILayout.Width(120));
                    }

                    GUILayout.Space(85);
                    GUILayout.Button(latestVersion, _packageInfoStyle);
                    GUILayout.Space(15);

                    action();
                    GUILayout.Space(15);
                }
            }
        }

        private void ImportDependency(AdapterSdkInfo adapterSdkInfo, string path)
        {
            if (!File.Exists(path))
            {
                File.WriteAllBytes(path, null);
                using var writer = new StreamWriter(path);
                writer.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?><dependencies></dependencies>");
                writer.Close();
            }

            string contentString;
            using (var reader = new StreamReader(path))
            {
                contentString = reader.ReadToEnd();
                reader.Close();
            }

            contentString = Regex.Replace(contentString, SspnetDependencyUtils.SpecCloseDependencies,
                adapterSdkInfo.unity_content + "\n" + SspnetDependencyUtils.SpecCloseDependencies);

            using (var writer = new StreamWriter(path))
            {
                writer.Write(contentString);
                writer.Close();
            }

            SspnetDependencyUtils.FormatXml(path);

            UpdateWindow();
        }

        private void AddUpdateDependencyActionIfCan(AdapterSdkInfo localAdapterSdkInfo, AdapterSdkInfo adapterSdkInfo,
            string path)
        {
            AddUpdateActionIfCan(localAdapterSdkInfo.version, adapterSdkInfo.version, () =>
            {
                SspnetDependencyUtils.ReplaceInFile(path, localAdapterSdkInfo.unity_content,
                    adapterSdkInfo.unity_content);
                SspnetDependencyUtils.FormatXml(path);
                UpdateWindow();
            });
        }

        private void AddUpdateCoreActionIfCan(PlatformSdk platformSdk)
        {
            const string path = SspnetDependencyUtils.NetworkConfigsPath + "SspnetCoreDependencies.xml";
            switch (platformSdk)
            {
                case PlatformSdk.Android:
                    AddUpdateActionIfCan(_localSdkInfo.android.version, _sdkInfo.android.version, () =>
                    {
                        SspnetDependencyUtils.ReplaceInFile(path, _localSdkInfo.android.unity_content,
                            _sdkInfo.android.unity_content);
                        SspnetDependencyUtils.FormatXml(path);
                        UpdateWindow();
                    });
                    break;
                case PlatformSdk.IOS:
                    AddUpdateActionIfCan(_localSdkInfo.ios.version, _sdkInfo.ios.version, () =>
                    {
                        SspnetDependencyUtils.ReplaceInFile(path, _localSdkInfo.ios.unity_content,
                            _sdkInfo.ios.unity_content);
                        SspnetDependencyUtils.FormatXml(path);
                        UpdateWindow();
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platformSdk), platformSdk, null);
            }
        }

        private void AddUpdatePluginActionIfCan(string localVersion, string latestVersion)
        {
            AddUpdateActionIfCan(localVersion, latestVersion, () => this.StartCoroutine(DownloadUnityPlugin()));
        }

        private void AddDependencyActions(AdapterSdkInfo localAdapterSdkInfo, AdapterSdkInfo adapterSdkInfo,
            string path)
        {
            if (localAdapterSdkInfo?.version == null)
            {
                AddImportActionIfCan(adapterSdkInfo, path);
            }
            else
            {
                AddRemoveActionIfCan(localAdapterSdkInfo, path);
                AddUpdateDependencyActionIfCan(localAdapterSdkInfo, adapterSdkInfo, path);
            }
        }

        private void AddRemoveActionIfCan(AdapterSdkInfo localAdapterSdkInfo, string path)
        {
            if (!GUILayout.Button(new GUIContent {text = "Remove"}, _btnFieldWidth)) return;
            SspnetDependencyUtils.ReplaceInFile(path, localAdapterSdkInfo.unity_content, "");
            SspnetDependencyUtils.FormatXml(path);
            UpdateWindow();
        }

        private void AddImportActionIfCan(AdapterSdkInfo adapterSdkInfo, string path)
        {
            var defaultColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;

            if (GUILayout.Button(new GUIContent {text = "Import"}, _btnFieldWidth))
            {
                ImportDependency(adapterSdkInfo, path);
            }

            GUI.backgroundColor = defaultColor;
        }

        private void AddUpdateActionIfCan(string localVersion, string latestVersion, Action action)
        {
            var compare = SspnetDependencyUtils.CompareVersions(localVersion, latestVersion);

            if (compare < 0)
            {
                var defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.green;

                if (GUILayout.Button(new GUIContent {text = "Update"}, _btnFieldWidth))
                {
                    action();
                }

                GUI.backgroundColor = defaultColor;
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Button(new GUIContent {text = "Update"}, _btnFieldWidth);
                GUI.enabled = true;
            }
        }

        private IEnumerator DownloadUnityPlugin()
        {
            yield return null;
            var ended = false;
            var cancelled = false;
            Exception error = null;
            int oldPercentage = 0, newPercentage = 0;

            var path = Path.Combine("temp", "download");

            _progress = 0.01f;

            _downloader = new WebClient {Encoding = Encoding.UTF8};
            _downloader.DownloadProgressChanged += (_, args) => { newPercentage = args.ProgressPercentage; };
            _downloader.DownloadFileCompleted += (_, args) =>
            {
                ended = true;
                cancelled = args.Cancelled;
                error = args.Error;
            };


            Debug.LogFormat("Downloading {0} to {1}", _pluginUrl, path);
            _downloader.DownloadFileAsync(new Uri(_pluginUrl), path);


            while (!ended)
            {
                Repaint();
                var percentage = oldPercentage;
                yield return new WaitUntil(() => ended || newPercentage > percentage);
                oldPercentage = newPercentage;
                _progress = oldPercentage / 100.0f;
            }

            if (error != null)
            {
                Debug.LogError(error);
                cancelled = true;
            }

            _downloader = null;
            _coroutine = null;
            _progress = 0;
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
    }
}

#endif