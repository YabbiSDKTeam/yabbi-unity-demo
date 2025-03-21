#if UNITY_EDITOR
using SspnetSDK.Editor.Utils;
using SspnetSDK.Editor.Windows;
using UnityEditor;
using UnityEngine;

namespace YabbiSDK.Editor.Utils
{
    public class YabbiEditorSettings : ScriptableObject
    {
        [MenuItem("Yabbi/Документация")]
        public static void OpenDocumentation()
        {
            Application.OpenURL("https://yabbi.gitbook.io/yabbi-documentation/unity-plugin");
        }

        [MenuItem("Yabbi/Официальный сайт")]
        public static void OpenPublisherUrl()
        {
            Application.OpenURL("https://yabbi.me/publishers");
        }

#if UNITY_2018_1_OR_NEWER
        [MenuItem("Yabbi/Управление зависимостями")]
        public static void ShowDependencyManager()
        {
            SspnetAdapterManager.ShowWindow(
                "https://sdkpkg.sspnet.tech/unity/yabbi/latest/yabbi-unity-plugin.unitypackage");
        }
#endif

        [MenuItem("Yabbi/Настройки плагина")]
        public static void ShowInternalSettings()
        {
            SspnetInternalSettings.ShowInternalSettings();
        }

        [MenuItem("Yabbi/Удалить плагин")]
        public static void RemovePlugin()
        {
            YabbiRemoveHelper.RemovePlugin();
        }

        [MenuItem("Yabbi/Удалить Consent Manager")]
        public static void RemoveConsentPlugin()
        {
            YabbiRemoveHelper.RemoveConsentManager();
        }
    }
}
#endif