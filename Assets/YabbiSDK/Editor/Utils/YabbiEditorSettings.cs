#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SspnetSDK.Editor.NetworkManager;
using SspnetSDK.Editor.Utils;

namespace YabbiSDK.Editor.Utils
{
    public class YabbiEditorSettings : ScriptableObject
    {
        [MenuItem("Yabbi/Документация")]
        public static void OpenDocumentation()
        {
            Application.OpenURL("https://mobileadx.gitbook.io/mobileadx/unity-plugin");
        }

        [MenuItem("Yabbi/Официальный сайт")]
        public static void OpenPublisherUrl()
        {
            Application.OpenURL("https://yabbi.me/publishers");
        }

#if UNITY_2018_1_OR_NEWER        
        [MenuItem("Yabbi/Управление зависимостями")]
        public static void ShowSdkManager()
        {
            SspnetAdapterManager.ShowSdkManager("https://github.com/YabbiSDKTeam/yabbi-unity-demo/releases/download/version/yabbi-unity-plugin.unitypackage");
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
