using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using marijnz.EditorCoroutines;
using SspnetSDK.Editor.Models;
using SspnetSDK.Editor.Unfiled;
using UnityEditor;
using UnityEngine;
using static System.String;

namespace SspnetSDK.Editor.Utils
{
    public abstract class GUIWindow : EditorWindow
    {
        protected readonly GUILayoutOption ButtontWidth = GUILayout.Width(60);
        private GUIStyle _headerInfoStyle;
        private GUIStyle _packageInfoStyle;
        protected GUIStyle LabelStyle;

        protected void Awake()
        {
            LabelStyle = new GUIStyle(EditorStyles.label)
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

        protected abstract void Reset();

        protected void SetDependenciesHeader()
        {
            using (new EditorGUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
            {
                GUILayout.Button("Name", _headerInfoStyle, GUILayout.Width(150));
                GUILayout.Space(25);
                GUILayout.Button("Current Version", _headerInfoStyle, GUILayout.Width(120));
                GUILayout.Space(90);
                GUILayout.Button("Latest Version", _headerInfoStyle);
                GUILayout.Button("Action", _headerInfoStyle, ButtontWidth);
                GUILayout.Button(Empty, _headerInfoStyle, GUILayout.Width(5));
            }
        }

        protected void SetDependencyRow(string dependencyName, string localVersion, string latestVersion, Action action)
        {
            using (new EditorGUILayout.VerticalScope("box"))
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
                        GUILayout.Button("   -   ", _packageInfoStyle, GUILayout.Width(120));
                    else
                        GUILayout.Button(localVersion, _packageInfoStyle, GUILayout.Width(120));

                    GUILayout.Space(85);
                    GUILayout.Button(latestVersion, _packageInfoStyle);
                    GUILayout.Space(15);

                    action();
                    GUILayout.Space(15);
                }
            }
        }

        protected void ImportDependency(AdapterSdkInfo adapterSdkInfo, string path)
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

            contentString = Regex.Replace(contentString, "</dependencies>",
                adapterSdkInfo.unity_content + "\n" + "</dependencies>");

            using (var writer = new StreamWriter(path))
            {
                writer.Write(contentString);
                writer.Close();
            }

            XmlUtilities.FormatXml(path);
            UpdateWindow();
        }

        protected abstract void UpdateWindow();


        protected abstract IEnumerator DownloadUnityPlugin();


        protected void AddDependencyActions(AdapterSdkInfo localAdapterSdkInfo, AdapterSdkInfo adapterSdkInfo,
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

        protected void AddImportActionIfCan(AdapterSdkInfo adapterSdkInfo, string path)
        {
            var defaultColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;

            if (GUILayout.Button(new GUIContent {text = "Import"}, ButtontWidth))
                ImportDependency(adapterSdkInfo, path);

            GUI.backgroundColor = defaultColor;
        }

        protected void AddRemoveActionIfCan(AdapterSdkInfo localAdapterSdkInfo, string path)
        {
            if (!GUILayout.Button(new GUIContent {text = "Remove"}, ButtontWidth)) return;
            FileUtils.ReplaceInFile(path, localAdapterSdkInfo.unity_content, "");
            XmlUtilities.FormatXml(path);
            UpdateWindow();
        }

        protected void AddUpdateDependencyActionIfCan(AdapterSdkInfo localAdapterSdkInfo, AdapterSdkInfo adapterSdkInfo,
            string path)
        {
            AddUpdateActionIfCan(localAdapterSdkInfo.version, adapterSdkInfo.version, () =>
            {
                FileUtils.ReplaceInFile(path, localAdapterSdkInfo.unity_content,
                    adapterSdkInfo.unity_content);
                XmlUtilities.FormatXml(path);
                UpdateWindow();
            });
        }

        protected void AddUpdatePluginActionIfCan(string localVersion, string latestVersion)
        {
            AddUpdateActionIfCan(localVersion, latestVersion, () => this.StartCoroutine(DownloadUnityPlugin()));
        }

        protected void AddUpdateActionIfCan(string localVersion, string latestVersion, Action action)
        {
            var compare = VersionUtils.CompareVersions(localVersion, latestVersion);

            if (compare < 0)
            {
                var defaultColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.green;

                if (GUILayout.Button(new GUIContent {text = "Update"}, ButtontWidth)) action();

                GUI.backgroundColor = defaultColor;
            }
            else
            {
                GUI.enabled = false;
                GUILayout.Button(new GUIContent {text = "Update"}, ButtontWidth);
                GUI.enabled = true;
            }
        }

        protected static void ShowDialog(EditorWindow editorWindow, string message)
        {
            EditorUtility.ClearProgressBar();
            var option = EditorUtility.DisplayDialog("Internal error",
                $"{message}. Please contact support.",
                "Ok");
            if (option) editorWindow.Close();
        }
    }
}