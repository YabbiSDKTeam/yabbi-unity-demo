#pragma warning disable 612
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using SspnetSDK.Editor.InternalResources;
using SspnetSDK.Editor.NetworkManager;

#pragma warning disable 618

namespace SspnetSDK.Editor.Utils
{

    [SuppressMessage("ReSharper", "NotAccessedField.Local")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "CollectionNeverQueried.Global")]
    public class SspnetInternalSettings : EditorWindow
    {

        private static List<string> SKAdNetworkIdentifiers = new();

        public static void ShowInternalSettings()
        {
            GetWindowWithRect(typeof(SspnetInternalSettings), new Rect(0, 0, 650, 200), true, "SDK Settings");
        }

        private void OnGUI()
        {

            #region Android Settings

            GUILayout.BeginHorizontal();


            using (new EditorGUILayout.VerticalScope("box", GUILayout.Width(200), GUILayout.Height(150)))
            {
                LabelField("Android Settings");
                GUILayout.Space(10);
                HeaderField("AndroidManifest.xml",
                    "https://mobileadx.gitbook.io/mobileadx/unity-plugin/ustanovka-i-nastroika");

                SspnetSettings.Instance.AccessCoarseLocationPermission = KeyRow("ACCESS_COARSE_LOCATION",
                    SspnetSettings.Instance.AccessCoarseLocationPermission);
                SspnetSettings.Instance.AccessFineLocationPermission = KeyRow("ACCESS_FINE_LOCATION",
                    SspnetSettings.Instance.AccessFineLocationPermission);

                GUILayout.Space(10);
                if (GUILayout.Button("Multidex", new GUIStyle(EditorStyles.label)
                    {
                        fontSize = 12,
                        fontStyle = FontStyle.Bold,
                        fixedHeight = 18
                    }, GUILayout.Width(100)))
                {
                    Application.OpenURL("https://developer.android.com/studio/build/multidex");
                }

                SspnetSettings.Instance.AndroidMultidex =
                    KeyRow("Enable multidex", SspnetSettings.Instance.AndroidMultidex);
                GUILayout.Space(12);
            }

            #endregion

            #region iOS Settings

            using (new EditorGUILayout.VerticalScope("box", GUILayout.Width(200), GUILayout.Height(150)))
            {
                LabelField("iOS Settings");
                GUILayout.Space(10);
                HeaderField("Info.plist",
                    "https://mobileadx.gitbook.io/mobileadx/unity-plugin/ustanovka-i-nastroika");

                SspnetSettings.Instance.NSUserTrackingUsageDescription = KeyRow("NSUserTrackingUsageDescription",
                    SspnetSettings.Instance.NSUserTrackingUsageDescription);
                SspnetSettings.Instance.NSLocationWhenInUseUsageDescription = KeyRow(
                    "NSLocationWhenInUseUsageDescription",
                    SspnetSettings.Instance.NSLocationWhenInUseUsageDescription);

                GUILayout.Space(10);
                if (GUILayout.Button("SKAdNetwork", new GUIStyle(EditorStyles.label)
                    {
                        fontSize = 12,
                        fontStyle = FontStyle.Bold,
                        fixedHeight = 18
                    }, GUILayout.ExpandWidth(true)))
                {
                    Application.OpenURL("https://developer.apple.com/documentation/storekit/skadnetwork");
                }

                GetSkaNetworkIds();
                if (SspnetSettings.Instance.IOSSkAdNetworkItemsList != null &&
                    SspnetSettings.Instance.IOSSkAdNetworkItemsList.Count > 0)
                {
                    SspnetSettings.Instance.IOSSkAdNetworkItemsList = null;
                    SspnetSettings.Instance.IOSSkAdNetworkItemsList = SKAdNetworkIdentifiers;
                }
                else
                {
                    SspnetSettings.Instance.IOSSkAdNetworkItemsList = SKAdNetworkIdentifiers;
                }

                GUILayout.Space(12);
            }

            GUILayout.EndHorizontal();

            #endregion

            SspnetSettings.Instance.SaveAsync();
        }

        private void GetSkaNetworkIds()
        {
            SspnetSettings.Instance.IOSSkAdNetworkItems =
                KeyRow("Add SKAdNetworkItems", SspnetSettings.Instance.IOSSkAdNetworkItems);

            if (SspnetSettings.Instance.IOSSkAdNetworkItems)
            {
                SKAdNetworkIdentifiers.Clear();

                var path = $"{SspnetDependencyUtils.InternalResourcesPath}skadnetworkids.json";

                var source = new StreamReader(path);
                var fileContents = source.ReadToEnd();

                var skaItems =
                    JsonHelper.FromJson<string>(JsonHelper.fixJson(fileContents));
                foreach (var skaItem in skaItems)
                {

                    if (!string.IsNullOrEmpty(skaItem))
                    {
                        SKAdNetworkIdentifiers?.Add(skaItem);
                    }
                }
            }
            else
            {
                SKAdNetworkIdentifiers.Clear();
            }

            
        }

        private static void LabelField(string label)
        {
            EditorGUILayout.LabelField(label, new GUIStyle(EditorStyles.label)
                {
                    fontSize = 15,
                    fontStyle = FontStyle.Bold
                },
                GUILayout.Height(20), GUILayout.Width(311));
            GUILayout.Space(2);
        }

        private static void HeaderField(string header, string url)
        {
            if (GUILayout.Button(header, new GUIStyle(EditorStyles.label)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    fixedHeight = 18
                }, GUILayout.Width(200)))
            {
                Application.OpenURL(url);
            }

            GUILayout.Space(2);
        }

        private static bool KeyRow(string fieldTitle, bool value)
        {
            GUILayout.Space(5);
            var originalValue = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 235;
            value = EditorGUILayout.Toggle(fieldTitle, value);
            EditorGUIUtility.labelWidth = originalValue;
            return value;
        }
    }
}
