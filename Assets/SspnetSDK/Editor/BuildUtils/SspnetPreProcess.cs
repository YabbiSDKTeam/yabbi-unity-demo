#if UNITY_ANDROID || UNITY_EDITOR
#if UNITY_2018_1_OR_NEWER
using System.IO;
using System.Text;
using System.Xml;
using SspnetSDK.Editor.InternalResources;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
#endif


namespace SspnetSDK.Editor.BuildUtils
{
    public class SspnetPreProcess :
#if UNITY_2018_1_OR_NEWER
        IPreprocessBuildWithReport
#else
        IPreprocessBuild
#endif
    {
        #region Constants

        //Templates in Unity Editor Data folder
        private const string GradleDefaultTemplatePath = "PlaybackEngines/AndroidPlayer/Tools/GradleTemplates";

        //Paths without leading Assets folder
        private const string AndroidPluginsPath = "Plugins/Android";
        private const string GradleTemplateName = "mainTemplate.gradle";

        //Manifest add lines
        private const string ManifestMutlidexApp = "androidx.multidex.MultiDexApplication";

        #endregion

#if UNITY_2018_1_OR_NEWER
        public void OnPreprocessBuild(BuildReport report)
#else
        public void OnPreprocessBuild(BuildTarget target, string path)
#endif

        {
            var manifestPath = Path.Combine(Application.dataPath,
                "Plugins/Android/sspnet.androidlib/AndroidManifest.xml");

            var androidManifest = new AndroidManifest(manifestPath);

            AddOptionalPermissions(manifestPath, androidManifest);
            EnableMultidex(manifestPath, androidManifest);

            androidManifest.Save();
        }

        private void EnableMultidex(string manifestPath, AndroidManifest androidManifest)
        {
            if (CheckContainsMultidex(manifestPath, ManifestMutlidexApp)) androidManifest.RemoveMultiDexApplication();

            if (SspnetSettings.Instance.AndroidMultidex)
            {
                if (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel22)
                {
                    androidManifest.AddMultiDexApplication();

                    if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                    {
                        if (!SspnetBuildUtils.isGradleEnabled()) new EnableGradle().fixProblem();

                        var customGradeScript = GetCustomGradleScriptPath();

                        if (string.IsNullOrEmpty(customGradeScript) || !File.Exists(customGradeScript))
                        {
                            if (File.Exists(GetDefaultGradleTemplate()))
                                new CopyGradleScriptAndEnableMultidex().fixProblem();
                        }
                        else
                        {
                            var settings = new ImportantGradleSettings(customGradeScript);
                            if (!settings.isMultiDexAddedCompletely())
                                new EnableMultidexInGradle(customGradeScript).fixProblem();

                            if (!settings.isJavaVersionIncluded())
                                new EnableJavaVersion(customGradeScript).fixProblem();
                        }
                    }
                }
            }
            else
            {
                androidManifest.RemoveMultiDexApplication();
            }
        }


        private void AddOptionalPermissions(string manifestPath, AndroidManifest androidManifest)
        {
            if (SspnetSettings.Instance.AccessCoarseLocationPermission)
                // if (!CheckContainsPermission(manifestPath, SspnetBuildUtils.CoarseLocation))
                // {
                androidManifest.SetPermission(SspnetBuildUtils.CoarseLocation);
            // }
            else
                // if (CheckContainsPermission(manifestPath, SspnetBuildUtils.CoarseLocation))
                // {
                androidManifest.RemovePermission(SspnetBuildUtils.CoarseLocation);
            // }
            if (SspnetSettings.Instance.AccessFineLocationPermission)
                // if (!CheckContainsPermission(manifestPath, SspnetBuildUtils.FineLocation))
                // {
                androidManifest.SetPermission(SspnetBuildUtils.FineLocation);
            // }
            else
                // if (CheckContainsPermission(manifestPath, SspnetBuildUtils.FineLocation))
                // {
                androidManifest.RemovePermission(SspnetBuildUtils.FineLocation);
            // }
        }

        private bool CheckContainsPermission(string manifestPath, string permission)
        {
            return GetContentString(manifestPath).Contains(permission);
        }

        private bool CheckContainsMultidex(string manifestPath, string multidex)
        {
            return GetContentString(manifestPath).Contains(multidex);
        }

        private static string GetDefaultGradleTemplate()
        {
            var defaultGradleTemplateFullName = SspnetBuildUtils.combinePaths(
                EditorApplication.applicationContentsPath,
                GradleDefaultTemplatePath,
                GradleTemplateName);
            if (File.Exists(defaultGradleTemplateFullName)) return defaultGradleTemplateFullName;
            var unixAppContentsPath =
                Path.GetDirectoryName(Path.GetDirectoryName(EditorApplication.applicationContentsPath));
            defaultGradleTemplateFullName = SspnetBuildUtils.combinePaths(unixAppContentsPath,
                GradleDefaultTemplatePath,
                GradleTemplateName);

            return defaultGradleTemplateFullName;
        }

        private static string GetContentString(string path)
        {
            using var reader = new StreamReader(path);
            var contentString = reader.ReadToEnd();
            reader.Close();

            return contentString;
        }

        private static string GetCustomGradleScriptPath()
        {
            var androidDirectory = new DirectoryInfo(Path.Combine("Assets", AndroidPluginsPath));
            var filePaths = androidDirectory.GetFiles("*.gradle");
            return filePaths.Length > 0
                ? Path.Combine(Path.Combine(Application.dataPath, AndroidPluginsPath), filePaths[0].Name)
                : null;
        }

        public int callbackOrder => 0;
    }

    internal class AndroidXmlDocument : XmlDocument
    {
        protected readonly string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";
        protected readonly string AndroidXmlToolsNamespace = "http://schemas.android.com/tools";
        private readonly string mPath;

        protected AndroidXmlDocument(string path)
        {
            mPath = path;
            using (var reader = new XmlTextReader(mPath))
            {
                reader.Read();
                Load(reader);
            }

            var nsMgr = new XmlNamespaceManager(NameTable);
            nsMgr.AddNamespace("android", AndroidXmlNamespace);
            nsMgr.AddNamespace("tools", AndroidXmlToolsNamespace);
        }

        public void Save()
        {
            SaveAs(mPath);
        }

        private void SaveAs(string path)
        {
            using var writer = new XmlTextWriter(path, new UTF8Encoding(false));
            writer.Formatting = Formatting.Indented;
            Save(writer);
        }
    }

    internal class AndroidManifest : AndroidXmlDocument
    {
        public AndroidManifest(string path) : base(path)
        {
        }

        private XmlAttribute CreateAndroidAttribute(string key, string value)
        {
            var attr = CreateAttribute("android", key, AndroidXmlNamespace);
            attr.Value = value;
            return attr;
        }

        internal void SetPermission(string permission)
        {
            var manifest = SelectSingleNode("/manifest");
            if (manifest == null) return;
            foreach (XmlNode child in manifest.SelectNodes("uses-permission"))
                for (var i = 0; i < child.Attributes?.Count; i++)
                    if (child.Attributes[i].Value.Equals(permission))
                        manifest.RemoveChild(child);
            // var attr = CreateAttribute("tools", "node", AndroidXmlToolsNamespace);
            // attr.Value = "remove";
            // child.Attributes.Remove(attr);
            // // var attr = CreateAttribute("tools", "node", AndroidXmlToolsNamespace);
            // // attr.Value = "remove";
            // // child.Attributes.Append(attr);
            var newChild = CreateElement("uses-permission");
            manifest.AppendChild(newChild);
            var newAttribute = CreateAndroidAttribute("name", permission);
            newChild.Attributes.Append(newAttribute);
        }

        internal void RemovePermission(string permission)
        {
            var manifest = SelectSingleNode("/manifest");
            if (manifest == null) return;
            foreach (XmlNode child in manifest.SelectNodes("uses-permission"))
                for (var i = 0; i < child.Attributes?.Count; i++)
                    if (child.Attributes[i].Value.Equals(permission))
                    {
                        var attr = CreateAttribute("tools", "node", AndroidXmlToolsNamespace);
                        attr.Value = "remove";
                        child.Attributes.Append(attr);
                    }
        }

        internal void RemoveMultiDexApplication()
        {
            var manifest = SelectSingleNode("/manifest/application");
            if (manifest == null) return;
            for (var i = 0; i < manifest.Attributes?.Count; i++)
                if (manifest.Attributes[i].Value.Equals("androidx.multidex.MultiDexApplication"))
                    manifest.Attributes.Remove(manifest.Attributes[i]);
        }

        internal void AddMultiDexApplication()
        {
            var manifest = SelectSingleNode("/manifest/application");
            manifest?.Attributes?.Append(CreateAndroidAttribute("name", "androidx.multidex.MultiDexApplication"));
        }
    }
}
#endif