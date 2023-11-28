#if UNITY_IPHONE || UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace SspnetSDK.Editor.Utils
{
    public class SspnetPostProcess : MonoBehaviour
    {
        [PostProcessBuild(100)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            if (target.ToString() == "iOS" || target.ToString() == "iPhone")
            {
                iOSPostprocessUtils.PrepareProject(path);
            }
        }
    }
}
#endif