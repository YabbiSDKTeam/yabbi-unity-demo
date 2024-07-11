#if UNITY_2018_1_OR_NEWER
using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEditor;
using UnityEngine;
using File = UnityEngine.Windows.File;
using UnityEngine.Networking;
using SspnetSDK.Editor.Models;
using marijnz.EditorCoroutines;
using SspnetSDK.Editor.Unfiled;
using SspnetSDK.Editor.Utils;
using static System.String;

#pragma warning disable 618

#pragma warning disable 612

namespace SspnetSDK.Editor.Windows
{
    public class SspnetAdapterManager : GUIWindow
    {
        #region Constants

        private const string PluginVersion = "1.3.0";
        private const string PluginRequest = "https://sdkapi.sspnet.tech/api/versions2.json";
        private const string NetworkConfigsPath = "Assets/SspnetSDK/Editor/NetworkConfigs/";
        private const string ExternalNetworkDependencies = "Assets/SspnetSDK/Editor/ExternalNetworkDependencies/";

        #endregion

        private static EditorCoroutines.EditorCoroutine _coroutine;

        private float _progress;
        private float _loading;
        private WebClient _downloader;

        private SdkInfo _sdkInfo;
        private SdkInfo _localSdkInfo;


        private Vector2 _scrollPosition;

        private static string _pluginUrl;

        public static void ShowWindow(string source)
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

        protected override void UpdateWindow()
        {
            Reset();
            _coroutine = this.StartCoroutine(GetSDKData());
            GUI.enabled = true;
            AssetDatabase.Refresh();
        }

        protected override void Reset()
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

            if (!EditorUtility.DisplayCancelableProgressBar("Dependency manager", "Loading...", 80f))
            {
            }

            #region Plugin

            using (var webRequest = UnityWebRequest.Get(PluginRequest))
            {
                yield return webRequest.SendWebRequest();
                var pages = PluginRequest.Split('/');
                var page = pages.Length - 1;
                if (webRequest.isNetworkError)
                {
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    ShowDialog(this, webRequest.error);
                }
                else
                {
                    if (IsNullOrEmpty(webRequest.downloadHandler.text))
                    {
                        ShowDialog(this, "Can't find plugin information");
                        yield break;
                    }

                    var response = JsonUtility.FromJson<SdkInfo>(webRequest.downloadHandler.text);
                    _sdkInfo = response;


                    if (_sdkInfo == null)
                    {
                        ShowDialog(this, "Can't find plugin information");
                        yield break;
                    }
                }
            }

            #endregion

            #region Internal

            var networkDependencies = GetLocalDependencies(NetworkConfigsPath);

            var info = new SdkInfo();
            var androidPlatformSdkInfo = new PlatformSdkInfo();
            var iosPlatformSdkInfo = new PlatformSdkInfo();

            if (networkDependencies != null)
                foreach (var networkDependencyFile in networkDependencies)
                {
                    var path = NetworkConfigsPath + networkDependencyFile.Name;
                    if (!File.Exists(path)) continue;
                    if (path.ToLower().Contains("sspnet") && path.ToLower().Contains("core"))
                    {
                        var iosAdapterSdkInfo = GetAdapterSdkInfo(path, NetworkConfigsPath,
                            PlatformSdk.IOS);
                        var androidAdapterSdkInfo = GetAdapterSdkInfo(path, NetworkConfigsPath,
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
                        var iosAdapterSdkInfo = GetAdapterSdkInfo(path, NetworkConfigsPath,
                            PlatformSdk.IOS);
                        var androidAdapterSdkInfo = GetAdapterSdkInfo(path, NetworkConfigsPath,
                            PlatformSdk.Android);

                        var adapterName = GetAdapterName(path, NetworkConfigsPath);

                        var externalNetworkDependencies =
                            GetLocalDependencies(ExternalNetworkDependencies);

                        if (externalNetworkDependencies != null)
                        {
                            var externalNetworks =
                                externalNetworkDependencies.Where(value =>
                                    value.Name.ToLower().Contains(adapterName.ToLower()));

                            foreach (var fileInfo in externalNetworks)
                            {
                                var externalDependencyPath =
                                    ExternalNetworkDependencies + fileInfo.Name;
                                // var androidExternalAdapterSdkInfo = GetAdapterSdkInfo(externalDependencyPath,
                                //     ExternalNetworkDependencies, PlatformSdk.Android);
                                // var iOSExternalAdapterSdkInfo = GetAdapterSdkInfo(externalDependencyPath,
                                //     ExternalNetworkDependencies, PlatformSdk.IOS);
                                //
                                // iosAdapterSdkInfo.adapters.Add(iOSExternalAdapterSdkInfo);
                                // androidAdapterSdkInfo.adapters.Add(androidExternalAdapterSdkInfo);
                            }
                        }

                        androidPlatformSdkInfo.networks.Add(androidAdapterSdkInfo);
                        iosPlatformSdkInfo.networks.Add(iosAdapterSdkInfo);
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
                            sdkInfo.unity_content = iOSDependencyUtils.GetiOSContent(path);
                            break;
                        }
                        case PlatformSdk.Android when elementName == "androidPackage" &&
                                                      parentElementName == "androidPackages":
                        {
                            var specName = reader.GetAttribute("spec");

                            if (specName == null) return false;

                            if (sdkInfo.version != null) break;
                            sdkInfo.version = AndroidDependencyUtils.GetAndroidDependencyVersion(specName);
                            sdkInfo.unity_content = AndroidDependencyUtils.GetAndroidContent(path);
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


        private void OnGUI()
        {
            if (!IsSdkInfoReady()) return;

            minSize = new Vector2(700, 650);
            maxSize = new Vector2(2000, 2000);
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);
            GUILayout.BeginVertical();

            #region Plugin

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Plugin", LabelStyle, GUILayout.Height(20));

            SetDependenciesHeader();
            using (new EditorGUILayout.VerticalScope("box"))
            {
                SetDependencyRow("Plugin", PluginVersion, _sdkInfo.unity.version,
                    () => AddUpdatePluginActionIfCan(PluginVersion, _sdkInfo.unity.version));
            }

            #endregion

            #region Android

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Android", LabelStyle, GUILayout.Height(20));
            GUILayout.Space(5);

            SetDependenciesHeader();

            using (new EditorGUILayout.VerticalScope("box"))
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
            EditorGUILayout.LabelField("iOS", LabelStyle, GUILayout.Height(20));
            GUILayout.Space(5);

            SetDependenciesHeader();

            if (_localSdkInfo != null)
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    SetDependencyRow("Core", _localSdkInfo.ios.version, _sdkInfo.ios.version,
                        () => AddUpdateCoreActionIfCan(PlatformSdk.IOS));
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
                return NetworkConfigsPath + adapterName + "Dependencies" + ".xml";
            }
            else
            {
                var adapterName = adapterSdkInfo.name.Replace(parentSdkInfo.name, "");
                var formatted = adapterName.First().ToString().ToUpper() + adapterName[1..];
                var parentAdapterName = parentSdkInfo.name.First().ToString().ToUpper() + parentSdkInfo.name[1..];
                return ExternalNetworkDependencies + "/" + parentAdapterName + formatted +
                       "Dependencies" + ".xml";
            }
        }


        private void AddUpdateCoreActionIfCan(PlatformSdk platformSdk)
        {
            const string path = NetworkConfigsPath + "SspnetCoreDependencies.xml";
            switch (platformSdk)
            {
                case PlatformSdk.Android:
                    AddUpdateActionIfCan(_localSdkInfo.android.version, _sdkInfo.android.version, () =>
                    {
                        FileUtils.ReplaceInFile(path, _localSdkInfo.android.unity_content,
                            _sdkInfo.android.unity_content);
                        XmlUtilities.FormatXml(path);
                        UpdateWindow();
                    });
                    break;
                case PlatformSdk.IOS:
                    AddUpdateActionIfCan(_localSdkInfo.ios.version, _sdkInfo.ios.version, () =>
                    {
                        FileUtils.ReplaceInFile(path, _localSdkInfo.ios.unity_content,
                            _sdkInfo.ios.unity_content);
                        XmlUtilities.FormatXml(path);
                        UpdateWindow();
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(platformSdk), platformSdk, null);
            }
        }

        protected override IEnumerator DownloadUnityPlugin()
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
                AssetDatabase.ImportPackage(path, true);
            else
                Debug.Log("Download terminated.");
        }
    }
}

#endif