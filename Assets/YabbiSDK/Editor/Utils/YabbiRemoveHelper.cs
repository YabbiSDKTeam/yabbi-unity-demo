#pragma warning disable 0649
using UnityEditor;
using System.Diagnostics.CodeAnalysis;
using SspnetSDK.Editor.Utils;

namespace YabbiSDK.Editor.Utils
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "RedundantJumpStatement")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class YabbiRemoveHelper
    {
        public static void RemovePlugin()
        {
            if (EditorUtility.DisplayDialog("Удаление Yabbi",
                    "Вы действительно хотите удалить Yabbi из проекта?",
                    "Да",
                    "Отмена"
                ))
            {
                RemoveHelper.RemovePlugin("YabbiSDK/Editor/InternalResources/remove_sdk_list.xml");
            }
        }

        public static void RemoveConsentManager()
        {
            if (EditorUtility.DisplayDialog("Удаление ConsentManager",
                    "Вы действительно хотите удалить ConsentManager из проекта?",
                    "Да",
                    "Отмена"))
            {
                RemoveHelper.RemoveConsentManager("YabbiSDK/ConsentManagerSDK/Editor/InternalResources/remove_consent_list.xml");
            }
        }
    }
}