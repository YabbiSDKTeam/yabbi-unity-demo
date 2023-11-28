#if UNITY_ANDROID || UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using SspnetSDK.Editor.Checkers;
using SspnetSDK.Editor.InternalResources;
using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
using UnityEngine.Android;
#endif
using UnityEngine;


namespace SspnetSDK.Editor.Utils
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
            if(CheckContainsMultidex(manifestPath, ManifestMutlidexApp))
            {
                androidManifest.RemoveMultiDexApplication();
            }

            if (SspnetSettings.Instance.AndroidMultidex)
            {
                if (PlayerSettings.Android.minSdkVersion < AndroidSdkVersions.AndroidApiLevel22)
                {
                    androidManifest.AddMultiDexApplication();

                    if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                    {
                        if (!SspnetUnityUtils.isGradleEnabled())
                        {
                            new EnableGradle().fixProblem();
                        }

                        var customGradeScript = GetCustomGradleScriptPath();
                        
                        if (string.IsNullOrEmpty(customGradeScript) || !File.Exists(customGradeScript))
                        {
                            if (File.Exists(GetDefaultGradleTemplate()))
                            {
                                new CopyGradleScriptAndEnableMultidex().fixProblem();
                            }
                        }
                        else
                        {
                            var settings = new ImportantGradleSettings(customGradeScript);
                            if (!settings.isMultiDexAddedCompletely())
                            {
                                new EnableMultidexInGradle(customGradeScript).fixProblem();
                            }
                            
                            if (!settings.isJavaVersionIncluded())
                            {
                                new EnableJavaVersion(customGradeScript).fixProblem();
                            }
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
            {
                if (!CheckContainsPermission(manifestPath, SspnetUnityUtils.CoarseLocation))
                {
                    androidManifest.SetPermission(SspnetUnityUtils.CoarseLocation);
                }
            }
            else
            {
                if (CheckContainsPermission(manifestPath, SspnetUnityUtils.CoarseLocation))
                {
                    androidManifest.RemovePermission(SspnetUnityUtils.CoarseLocation);
                }
            }

            if (SspnetSettings.Instance.AccessFineLocationPermission)
            {
                if (!CheckContainsPermission(manifestPath, SspnetUnityUtils.FineLocation))
                {
                    androidManifest.SetPermission(SspnetUnityUtils.FineLocation);
                }
            }
            else
            {
                if (CheckContainsPermission(manifestPath, SspnetUnityUtils.FineLocation))
                {
                    androidManifest.RemovePermission(SspnetUnityUtils.FineLocation);
                }
            }
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
            var defaultGradleTemplateFullName = SspnetUnityUtils.combinePaths(
                EditorApplication.applicationContentsPath,
                GradleDefaultTemplatePath,
                GradleTemplateName);
            if (File.Exists(defaultGradleTemplateFullName)) return defaultGradleTemplateFullName;
            var unixAppContentsPath =
                Path.GetDirectoryName(Path.GetDirectoryName(EditorApplication.applicationContentsPath));
            defaultGradleTemplateFullName = SspnetUnityUtils.combinePaths(unixAppContentsPath,
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
        private readonly string mPath;
        protected readonly string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";

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
            var child = CreateElement("uses-permission");
            manifest.AppendChild(child);
            var newAttribute = CreateAndroidAttribute("name", permission);
            child.Attributes.Append(newAttribute);
        }

        internal void RemovePermission(string permission)
        {
            var manifest = SelectSingleNode("/manifest");
            if (manifest == null) return;
            foreach (XmlNode child in manifest.SelectNodes("uses-permission"))
            {
                for (var i = 0; i < child.Attributes?.Count; i++)
                {
                    if (child.Attributes[i].Value.Equals(permission))
                    {
                        manifest.RemoveChild(child);
                    }
                }
            }
        }

        internal void RemoveMultiDexApplication()
        {
            var manifest = SelectSingleNode("/manifest/application");
            if (manifest == null) return;
            for (var i = 0; i < manifest.Attributes?.Count; i++)
            {
                if (manifest.Attributes[i].Value.Equals("androidx.multidex.MultiDexApplication"))
                {
                    manifest.Attributes.Remove(manifest.Attributes[i]);
                }
            }
        }

        internal void AddMultiDexApplication()
        {
            var manifest = SelectSingleNode("/manifest/application");
            manifest?.Attributes?.Append(CreateAndroidAttribute("name", "androidx.multidex.MultiDexApplication"));
        }
    }

    internal class EnableJavaVersion : FixProblemInstruction
    {
        private readonly string path;

        public EnableJavaVersion(string gradleScriptPath) : base("Java version isn't included to mainTemplate.gradle",
            true)
        {
            path = gradleScriptPath;
        }

        public override void fixProblem()
        {
            var settings = new ImportantGradleSettings(path);
            var leadingWhitespaces = "    ";
            const string additionalWhiteSpaces = "";
            var modifiedGradle = "";

            var gradleScript = new StreamReader(path);

            while (gradleScript.ReadLine() is { } line)
            {
                if (line.Contains(MultidexActivator.GRAFLE_DEFAULT_CONFIG))
                {
                    if (!settings.compileOptions)
                    {
                        modifiedGradle += additionalWhiteSpaces + leadingWhitespaces +
                                          MultidexActivator.COMPILE_OPTIONS + Environment.NewLine;
                    }

                    if (!settings.sourceCapability)
                    {
                        modifiedGradle += leadingWhitespaces + leadingWhitespaces +
                                          MultidexActivator.GRADLE_SOURCE_CAPABILITY
                                          + MultidexActivator.GRADLE_JAVA_VERSION_1_8 + Environment.NewLine;
                    }

                    if (!settings.targetCapability)
                    {
                        modifiedGradle += leadingWhitespaces + leadingWhitespaces +
                                          MultidexActivator.GRADLE_TARGET_CAPATILITY
                                          + MultidexActivator.GRADLE_JAVA_VERSION_1_8 + Environment.NewLine;
                    }

                    if (!settings.targetCapability)
                    {
                        modifiedGradle += leadingWhitespaces + "}" + Environment.NewLine;
                    }

                    if (!settings.targetCapability)
                    {
                        modifiedGradle += leadingWhitespaces + Environment.NewLine;
                    }
                }

                modifiedGradle += line + Environment.NewLine;
                leadingWhitespaces = Regex.Match(line, "^\\s*").Value;
            }

            gradleScript.Close();
            File.WriteAllText(path, modifiedGradle);
            AssetDatabase.ImportAsset(SspnetUnityUtils.absolute2Relative(path), ImportAssetOptions.ForceUpdate);
        }
    }

    internal class CopyGradleScriptAndEnableMultidex : FixProblemInstruction
    {
        public CopyGradleScriptAndEnableMultidex() : base("Assets/Plugins/Android/mainTemplate.gradle not found.\n" +
                                                          "(required if you aren't going to export your project to Android Studio or Eclipse)",
            true)
        {
        }

        public override void fixProblem()
        {
            //EditorApplication.applicationContentsPath is different for macos and win. need to fix to reach manifest and gradle templates 
            var defaultGradleTemplateFullName = MultidexActivator.getDefaultGradleTemplate();

            var destGradleScriptFullName = SspnetUnityUtils.combinePaths(Application.dataPath,
                MultidexActivator.androidPluginsPath,
                MultidexActivator.gradleTemplateName);
            //Prefer to use build.gradle from the box. Just in case.
            if (File.Exists(defaultGradleTemplateFullName))
            {
                File.Copy(defaultGradleTemplateFullName, destGradleScriptFullName);
            }

            AssetDatabase.ImportAsset(SspnetUnityUtils.absolute2Relative(destGradleScriptFullName),
                ImportAssetOptions.ForceUpdate);

            //There are no multidex settings in default build.gradle but they can add that stuff.
            var settings = new ImportantGradleSettings(destGradleScriptFullName);

            if (!settings.isMultiDexAddedCompletely())
                new EnableMultidexInGradle(destGradleScriptFullName).fixProblem();
        }
    }

    internal class EnableMultidexInGradle : FixProblemInstruction
    {
        private readonly string path;

        public EnableMultidexInGradle(string gradleScriptPath) : base(
            "Multidex isn't enabled. mainTemplate.gradle should be edited " +
            "according to the official documentation:\nhttps://developer.android.com/studio/build/multidex", true)
        {
            path = gradleScriptPath;
        }

        public override void fixProblem()
        {
            var settings = new ImportantGradleSettings(path);
            var leadingWhitespaces = "";
            var prevLine = "";
            var modifiedGradle = "";
            var gradleScript = new StreamReader(path);


            var multidexDependency = MultidexActivator.GRADLE_IMPLEMENTATION +
                                     MultidexActivator.GRADLE_MULTIDEX_DEPENDENCY;


            while (gradleScript.ReadLine() is { } line)
            {
                if (!settings.multidexDependencyPresented && line.Contains(MultidexActivator.GRADLE_DEPENDENCIES))
                {
                    modifiedGradle += leadingWhitespaces + multidexDependency + Environment.NewLine;
                }

                if (!settings.multidexEnabled && line.Contains(MultidexActivator.GRADLE_APP_ID))
                {
                    modifiedGradle += leadingWhitespaces + MultidexActivator.GRADLE_MULTIDEX_ENABLE +
                                      Environment.NewLine;
                }

                if (settings.deprecatedProguardPresented && line.Contains(MultidexActivator.GRADLE_USE_PROGUARD))
                {
                    continue;
                }

                modifiedGradle += line + Environment.NewLine;
                leadingWhitespaces = Regex.Match(line, "^\\s*").Value;
                if (line.Contains("repositories") && prevLine.Contains("allprojects") &&
                    !settings.googleRepositoryPresented)
                {
                    leadingWhitespaces += leadingWhitespaces;
                    modifiedGradle += leadingWhitespaces + MultidexActivator.GRADLE_GOOGLE_REPOSITORY_COMPAT +
                                      Environment.NewLine;
                }

                prevLine = line;
            }

            gradleScript.Close();
            File.WriteAllText(path, modifiedGradle);
            AssetDatabase.ImportAsset(SspnetUnityUtils.absolute2Relative(path), ImportAssetOptions.ForceUpdate);
        }
    }
}
#endif
