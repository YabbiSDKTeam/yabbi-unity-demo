#if UNITY_IPHONE
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace SspnetSDK.Editor.BuildUtils
{
    public class SspnetPostProcess : MonoBehaviour
    {
        [PostProcessBuild(100)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            if (target.ToString() == "iOS" || target.ToString() == "iPhone") iOSPostProcessUtils.PrepareProject(path);
        }
    }
}
#endif